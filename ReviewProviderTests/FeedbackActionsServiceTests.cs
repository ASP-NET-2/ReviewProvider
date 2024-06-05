using Infrastructure.Data.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Infrastructure.Models.EntityModels;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ReviewProviderTests;

public class FeedbackActionsServiceTests
{
    private readonly FeedbackActionsService _feedbackActionsService;

    private readonly UserFeedbackRepository _userFeedbackRepo;
    private readonly Mock<ReviewRepository> _reviewRepo;
    private readonly Mock<RatingRepository> _ratingRepo;
    private readonly ProductFeedbackRepository _productFeedbackRepo;
    private readonly Mock<IDbContextFactory<IdentityDataContext>> _mockIdentityContextFactory;
    private readonly Mock<IDbContextFactory<FeedbackItemsDataContext>> _mockFeedbackContextFactory;
    private readonly UserRepository _userRepository;
    private readonly Mock<ILogger<FeedbackActionsService>> _logger;

    private DbContextOptions<IdentityDataContext> _identityOptions;
    private DbContextOptions<FeedbackItemsDataContext> _feedbackOptions;
    //private SqliteConnection _connection;

    private readonly string _testUserId = "";

    public FeedbackActionsServiceTests()
    {
        _identityOptions = CreateIdentityDbOptions("default_identity");
        _feedbackOptions = CreateFeedbackItemsDbOptions("default_feedback");

        _userFeedbackRepo = new UserFeedbackRepository(null!);
        _reviewRepo = new Mock<ReviewRepository>(null!); _reviewRepo.CallBase = true;
        _ratingRepo = new Mock<RatingRepository>(null!); _ratingRepo.CallBase = true;
        _productFeedbackRepo = new ProductFeedbackRepository(null!);
        _userRepository = new UserRepository(null!);

        _mockIdentityContextFactory = new Mock<IDbContextFactory<IdentityDataContext>>();
        _mockIdentityContextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken token) =>
            {
                var ctx = CreateIdentityDataContext();
                return ctx;
            });

        SetupUsersInDatabase(out _testUserId);

        _mockFeedbackContextFactory = new Mock<IDbContextFactory<FeedbackItemsDataContext>>();
        _mockFeedbackContextFactory.Setup(x => x.CreateDbContextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((CancellationToken token) =>
            {
                var ctx = CreateFeedbackItemsDataContext();
                return ctx;
            });

        _logger = new Mock<ILogger<FeedbackActionsService>>();

        _feedbackActionsService = new FeedbackActionsService(_userFeedbackRepo, _reviewRepo.Object,
            _ratingRepo.Object, _productFeedbackRepo, _mockFeedbackContextFactory.Object, _logger.Object,
            _mockIdentityContextFactory.Object, _userRepository);
    }

    #region Setups

    private DbContextOptions<IdentityDataContext> CreateIdentityDbOptions(string name) =>
    new DbContextOptionsBuilder<IdentityDataContext>()
    .UseInMemoryDatabase(name).Options;

    private DbContextOptions<FeedbackItemsDataContext> CreateFeedbackItemsDbOptions(string name) =>
        new DbContextOptionsBuilder<FeedbackItemsDataContext>()
        .UseInMemoryDatabase(name).Options;

    private IdentityDataContext CreateIdentityDataContext(bool ensureFresh = false)
    {
        var ctx = new IdentityDataContext(_identityOptions);
        if (ensureFresh)
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
        }

        return ctx;
    }

    private FeedbackItemsDataContext CreateFeedbackItemsDataContext(bool ensureFresh = false)
    {
        var ctx = new FeedbackItemsDataContext(_feedbackOptions);
        if (ensureFresh)
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();
        }

        return ctx;
    }

    private void SetupUsersInDatabase(out string userId)
    {
        var userEntity = new UserEntity();
        userId = userEntity.Id;

        using var ctx = CreateIdentityDataContext();
        ctx.Users.Add(userEntity);
        ctx.SaveChanges();
    }

    #endregion

    #region Helpers

    private async Task Helper_CreateReview(FeedbackItemsDataContext ctx, string userId, string productId, string reviewTitle, string reviewText)
    {
        var userFeedback = await _userFeedbackRepo.GetAsync(ctx, 
            x => x.ProductId == productId && x.UserId == userId && x.Review != null);  

        if (userFeedback == null)
        {
            var reviewEntity = await _reviewRepo.Object.CreateAsync(ctx, new ReviewEntity
            {
                ReviewTitle = reviewTitle,
                ReviewText = reviewText,
            });


            userFeedback = await _userFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId && x.UserId == userId);
            if (userFeedback != null)
            {
                userFeedback.Review = reviewEntity;
                await _userFeedbackRepo.UpdateAsync(ctx, userFeedback, true);
            }
            else
            {
                userFeedback = await _userFeedbackRepo.CreateAsync(ctx, new UserFeedbackEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Review = reviewEntity,
                });
            }

            var productFeedback = await _productFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId);
            if (productFeedback != null)
            {
                var result = productFeedback.UserFeedbacks.FirstOrDefault(x => 
                x.ProductId == productId && x.UserId == userId);

                if (result != null)
                {
                    productFeedback.UserFeedbacks.Remove(result);
                }

                productFeedback.UserFeedbacks.Add(userFeedback);
                productFeedback.ReviewCount++;
                await _productFeedbackRepo.UpdateAsync(ctx, productFeedback, true);
            }
            else
            {
                productFeedback = await _productFeedbackRepo.CreateAsync(ctx, new ProductFeedbackEntity
                {
                    ProductId = productId,
                    UserFeedbacks = { userFeedback },
                    ReviewCount = 1,
                });
            }
        }

    }

    private async Task Helper_CreateRating(FeedbackItemsDataContext ctx, string userId, string productId, decimal rating)
    {
        var userFeedback = await _userFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId && x.UserId == userId && x.Rating != null);
        if (userFeedback == null)
        {
            var ratingEntity = await _ratingRepo.Object.CreateAsync(ctx, new RatingEntity
            {
                Rating = rating,
            });

            userFeedback = await _userFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId && x.UserId == userId);
            if (userFeedback != null)
            {
                userFeedback.Rating = ratingEntity;
                await _userFeedbackRepo.UpdateAsync(ctx, userFeedback, true);
            }
            else
            {
                userFeedback = await _userFeedbackRepo.CreateAsync(ctx, new UserFeedbackEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Rating = ratingEntity,
                });
            }

            var productFeedback = await _productFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId);
            if (productFeedback != null)
            {
                var result = productFeedback.UserFeedbacks.FirstOrDefault(x => 
                x.ProductId == productId && x.UserId == userId);

                if (result != null)
                {
                    productFeedback.UserFeedbacks.Remove(result);
                }

                productFeedback.UserFeedbacks.Add(userFeedback);
                productFeedback.RatingCount++;
                await _productFeedbackRepo.UpdateAsync(ctx, productFeedback, true);
            }
            else
            {
                productFeedback = await _productFeedbackRepo.CreateAsync(ctx, new ProductFeedbackEntity
                {
                    ProductId = productId,
                    UserFeedbacks = { userFeedback },
                    RatingCount = 1,
                    AverageRating = 1.0m,
                });
            }
        }
    }

    #endregion

    #region ReviewProductAsync

    [Fact]
    public async Task ReviewProductAsync_Should_OnSuccessfulStore_PutRelevantEntitiesInDatabase_AndReturnTrue()
    {
        // ~==ARRANGE==~

        // For making sure that the database is cleaned later...
        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        var model = new ReviewModel
        {
            ReviewTitle = "Title",
            ReviewText = "Text Oh Yeah",
        };

        string productId = Guid.NewGuid().ToString();

        // ~==ACT==~

        bool result = await _feedbackActionsService.ReviewProductAsync(productId, _testUserId, model);

        ProductFeedbackEntity? feedbackEntity = null;
        UserFeedbackEntity? userFeedback = null;
        ReviewEntity? reviewEntity = null;

        feedbackEntity = await _productFeedbackRepo.GetAsync(feedbackCtx, x => x.ProductId == productId, true);
        if (feedbackEntity != null)
        {
            userFeedback = feedbackEntity.UserFeedbacks.FirstOrDefault(x => x.UserId == _testUserId);
            if (userFeedback != null)
            {
                reviewEntity = userFeedback.Review;
            }
        }

        // ~==ASSERT==~
        Assert.True(result, "Result was false.");
        
        Assert.True(feedbackEntity != null, "Function did not create a ProductFeedbackEntity for the requested product if one didn't exist.");
        Assert.True(userFeedback != null, "Function did not create a UserFeedbackEntity for the requested product/user if one didn't exist.");
        Assert.True(reviewEntity != null, "No review was stored in the database.");
    }

    [Fact]
    public async Task ReviewProductAsync_Should_IfModelInvalid_ReturnFalse()
    {
        // ~==ARRANGE==~

        // For making sure that the database is cleaned later...
        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        ReviewModel model = null!;

        string productId = Guid.NewGuid().ToString();

        // ~==ACT==~

        bool result = await _feedbackActionsService.ReviewProductAsync(productId, _testUserId, model);

        // ~==ASSERT==~

        Assert.False(result, "Result was true.");
    }

    [Fact]
    public async Task ReviewProductAsync_Should_IfReviewAlreadyExists_UpdateExistingReviewEntity()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        await Helper_CreateReview(feedbackCtx, _testUserId, productId, "Review yeah", "Review Texticles");

        var model = new ReviewModel
        {
            ReviewTitle = "Title",
            ReviewText = "Text Oh Yeah",
        };

        // ~==ACT==~

        bool result = await _feedbackActionsService.ReviewProductAsync(productId, _testUserId, model);

        // ~==ASSERT==~

        _reviewRepo.Verify(x => x.UpdateAsync(It.IsAny<FeedbackItemsDataContext>(), It.IsAny<ReviewEntity>(), It.IsAny<bool>()), Times.Exactly(1));
    }

    #endregion

    #region RateProduct

    [Fact]
    public async Task RateProductAsync_Should_OnSuccessfulStore_PutRelevantEntitiesInDatabase_AndReturnTrue()
    {
        // ~==ARRANGE==~

        // For making sure that the database is cleaned later...
        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        var model = new RatingModel
        {
            Rating = 1.0m,
        };

        string productId = Guid.NewGuid().ToString();

        // ~==ACT==~

        bool result = await _feedbackActionsService.RateProductAsync(productId, _testUserId, model);

        ProductFeedbackEntity? feedbackEntity = null;
        UserFeedbackEntity? userFeedback = null;
        RatingEntity? ratingEntity = null;

        feedbackEntity = await _productFeedbackRepo.GetAsync(feedbackCtx, x => x.ProductId == productId, true);
        if (feedbackEntity != null)
        {
            userFeedback = feedbackEntity.UserFeedbacks.FirstOrDefault(x => x.UserId == _testUserId);
            if (userFeedback != null)
            {
                ratingEntity = userFeedback.Rating;
            }
        }

        // ~==ASSERT==~

        Assert.True(result, "Result was false.");
        Assert.True(feedbackEntity != null, "Function did not create a ProductFeedbackEntity for the requested product if one didn't exist.");
        Assert.True(userFeedback != null, "Function did not create a UserFeedbackEntity for the requested product/user if one didn't exist.");
        Assert.True(ratingEntity != null, "No rating was stored in the database.");
    }

    [Fact]
    public async Task RateProductAsync_Should_IfModelInvalid_ReturnFalse()
    {
        // ~==ARRANGE==~

        // For making sure that the database is cleaned later...
        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        RatingModel model = null!;

        string productId = Guid.NewGuid().ToString();

        // ~==ACT==~

        bool result = await _feedbackActionsService.RateProductAsync(productId, _testUserId, model);

        // ~==ASSERT==~

        Assert.False(result, "Result was true.");
    }

    [Fact]
    public async Task RateProductAsync_Should_IfReviewAlreadyExists_UpdateExistingReviewEntity()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        await Helper_CreateRating(feedbackCtx, _testUserId, productId, 1.0m);

        var model = new RatingModel
        {
            Rating = 0.8m,
        };

        // ~==ACT==~

        bool result = await _feedbackActionsService.RateProductAsync(productId, _testUserId, model);

        // ~==ASSERT==~

        _ratingRepo.Verify(x => x.UpdateAsync(It.IsAny<FeedbackItemsDataContext>(), It.IsAny<RatingEntity>(), It.IsAny<bool>()), Times.Exactly(1));
    }

    #endregion

    #region GetUserFeedback

    [Fact]
    public async Task GetUserFeedbackAsync_Should_ReturnReview_IfUserHasPostedOne()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        string title = "Review yeah";
        string text = "Review Texticles";

        await Helper_CreateReview(feedbackCtx, _testUserId, productId, title, text);

        // ~==ACT==~

        var result = await _feedbackActionsService.GetUserFeedbackAsync(productId, _testUserId);

        // ~==ASSERT==~
        
        Assert.NotNull(result);
        Assert.NotNull(result.Review);
        Assert.True(result.Review != null && result.Review.ReviewTitle == title 
            && result.Review.ReviewText == text);
    }

    [Fact]
    public async Task GetUserFeedbackAsync_Should_ReturnRating_IfUserHasPostedOne()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);
        
        string productId = Guid.NewGuid().ToString();

        decimal rating = 0.6m;

        await Helper_CreateRating(feedbackCtx, _testUserId, productId, rating);

        // ~==ACT==~

        var result = await _feedbackActionsService.GetUserFeedbackAsync(productId, _testUserId);

        // ~==ASSERT==~

        Assert.NotNull(result);
        Assert.NotNull(result.Rating);
        Assert.True(result.Rating == rating, "Rating was not the same.");
    }

    #endregion

    #region GetAllUserFeedbacks

    [Fact]
    public async Task GetUserFeedbacksAsync_Should_IfNoUserSpecified_ReturnAllFeedbacksOfProduct()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId1 = Guid.NewGuid().ToString();

        for (int i = 0; i < 10; i++)
        {
            string fakeUserId = Guid.NewGuid().ToString();
            string fakeTitle = Guid.NewGuid().ToString();
            string fakeReview = Guid.NewGuid().ToString();

            await Helper_CreateReview(feedbackCtx, fakeUserId, productId1, fakeTitle, fakeReview);
        }

        string productId2 = Guid.NewGuid().ToString();

        for (int i = 0; i < 10; i++)
        {
            string fakeUserId = Guid.NewGuid().ToString();
            string fakeTitle = Guid.NewGuid().ToString();
            string fakeReview = Guid.NewGuid().ToString();

            await Helper_CreateReview(feedbackCtx, fakeUserId, productId2, fakeTitle, fakeReview);
        }

        // ~==ACT==~

        var result = await _feedbackActionsService.GetUserFeedbacksAsync(productId1, null, null, null, true, true);

        bool allHaveSameProductId = true;
        foreach (var item in result) 
        {
            if (item.ProductId != productId1)
            {
                allHaveSameProductId = false;
                break;
            }
        }

        // ~==ASSERT==~

        Assert.True(allHaveSameProductId, "A review of another product managed to sneak into the results.");
    }

    [Fact]
    public async Task GetUserFeedbacksAsync_Should_IfNoProductSpecified_ReturnAllFeedbacksByUser()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        for (int i = 0; i < 10; i++)
        {
            string fakeProductId = Guid.NewGuid().ToString();
            string fakeTitle = Guid.NewGuid().ToString();
            string fakeReview = Guid.NewGuid().ToString();

            await Helper_CreateReview(feedbackCtx, _testUserId, fakeProductId, fakeTitle, fakeReview);
        }

        string testUserId2 = Guid.NewGuid().ToString();

        for (int i = 0; i < 10; i++)
        {
            string fakeProductId = Guid.NewGuid().ToString();
            string fakeTitle = Guid.NewGuid().ToString();
            string fakeReview = Guid.NewGuid().ToString();

            await Helper_CreateReview(feedbackCtx, testUserId2, fakeProductId, fakeTitle, fakeReview);
        }

        // ~==ACT==~

        var result = await _feedbackActionsService.GetUserFeedbacksAsync(null, _testUserId, null, null, true, true);

        bool allHaveSameUserId = true;
        foreach (var item in result)
        {
            if (item.UserId != _testUserId)
            {
                allHaveSameUserId = false;
                break;
            }
        }

        // ~==ASSERT==~

        Assert.True(allHaveSameUserId, "A review by another user managed to sneak into the results.");
    }

    [Fact]
    public async Task GetUserFeedbacksAsync_Should_IfStartIndexAndTakeSpecified_SkipAndTakeResults()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        const int skipCount = 6;
        const int takeCount = 4;
        string skipReview = "";
        string finalReview = "";

        for (int i = 0; i < 17; i++)
        {
            string fakeTitle = Guid.NewGuid().ToString();
            string fakeReview = Guid.NewGuid().ToString();
            string fakeUserId = Guid.NewGuid().ToString();

            if (i == skipCount)
            {
                skipReview = fakeReview;
            }
            else if (i == skipCount + takeCount - 1)
            {
                finalReview = fakeReview;
            }

            await Helper_CreateReview(feedbackCtx, fakeUserId, productId, fakeTitle, fakeReview);
        }

        // ~==ACT==~

        var result = await _feedbackActionsService.GetUserFeedbacksAsync(productId, null, skipCount, takeCount, true, true);

        // ~==ASSERT==~

        var r = result.First();

        // Read in reverse order because descending
        Assert.True(result.Last().Review!.ReviewText == skipReview, "Oldest review is not the one that should be skipped to.");
        Assert.True(result.First().Review!.ReviewText == finalReview, "Newest review is not the last one to be taken.");
    }

    #endregion

    #region DeleteReview

    [Fact]
    public async Task DeleteReviewAsync_Should_WhenReviewDeleted_NotExistInDatabase()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        string reviewText = "Youuu";
        string reviewTitle = "Hey";

        await Helper_CreateReview(feedbackCtx, _testUserId, productId, reviewTitle, reviewText);

        var resultBeforeDeletion = await _reviewRepo.Object.GetAsync(feedbackCtx, x => x.ReviewText == reviewText && x.ReviewTitle == reviewTitle);

        // ~==ACT==~

        var result = await _feedbackActionsService.DeleteReviewAsync(productId, _testUserId);

        // ~==ASSERT==~

        Assert.NotNull(resultBeforeDeletion);
        Assert.Null(await _reviewRepo.Object.GetAsync(feedbackCtx, x => x.ReviewText == reviewText && x.ReviewTitle == reviewTitle));
    }

    [Fact]
    public async Task DeleteReviewAsync_Should_WhenBothRatingAndReviewDeleted_DeleteUserFeedbackEntity()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        string reviewTitle = "Hey";
        string reviewText = "Youuu";
        decimal rating = 0.8m;

        await Helper_CreateReview(feedbackCtx, _testUserId, productId, reviewTitle, reviewText);
        await Helper_CreateRating(feedbackCtx, _testUserId, productId, rating);

        var resultBeforeDeletion = await _userFeedbackRepo.GetAsync(feedbackCtx, x => x.ProductId == productId && x.UserId == _testUserId);

        // ~==ACT==~

        bool ratingResult = await _feedbackActionsService.DeleteRatingAsync(productId, _testUserId);
        bool reviewResult = await _feedbackActionsService.DeleteReviewAsync(productId, _testUserId);

        // ~==ASSERT==~

        Assert.True(ratingResult); 
        Assert.True(reviewResult);
        Assert.NotNull(resultBeforeDeletion);
        Assert.Null(await _userFeedbackRepo.GetAsync(feedbackCtx, x => x.ProductId == productId && x.UserId == _testUserId));
    }

    #endregion

    #region DeleteRating

    [Fact]
    public async Task DeleteRatingAsync_Should_WhenReviewDeleted_NotExistInDatabase()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        decimal rating = 0.8m;
        
        await Helper_CreateRating(feedbackCtx, _testUserId, productId, rating);

        var resultBeforeDeletion = await _ratingRepo.Object.GetAsync(feedbackCtx, x => x.Rating == rating);

        // ~==ACT==~

        var result = await _feedbackActionsService.DeleteRatingAsync(productId, _testUserId);

        // ~==ASSERT==~

        Assert.NotNull(resultBeforeDeletion);
        Assert.Null(await _ratingRepo.Object.GetAsync(feedbackCtx, x => x.Rating == rating));
    }

    [Fact]
    public async Task DeleteRatingAsync_Should_WhenBothRatingAndReviewDeleted_DeleteUserFeedbackEntity()
    {
        // ~==ARRANGE==~

        using var feedbackCtx = CreateFeedbackItemsDataContext(true);

        string productId = Guid.NewGuid().ToString();

        string reviewTitle = "Hey";
        string reviewText = "Youuu";
        decimal rating = 0.8m;

        await Helper_CreateReview(feedbackCtx, _testUserId, productId, reviewTitle, reviewText);
        await Helper_CreateRating(feedbackCtx, _testUserId, productId, rating);

        var resultBeforeDeletion = await _userFeedbackRepo.GetAsync(feedbackCtx, x => x.ProductId == productId && x.UserId == _testUserId);

        // ~==ACT==~

        // switched places compared to deletereviewasync
        bool reviewResult = await _feedbackActionsService.DeleteReviewAsync(productId, _testUserId);
        bool ratingResult = await _feedbackActionsService.DeleteRatingAsync(productId, _testUserId);

        // ~==ASSERT==~

        Assert.True(ratingResult);
        Assert.True(reviewResult);
        Assert.NotNull(resultBeforeDeletion);
        Assert.Null(await _userFeedbackRepo.GetAsync(feedbackCtx, x => x.ProductId == productId && x.UserId == _testUserId));
    }

    #endregion
}