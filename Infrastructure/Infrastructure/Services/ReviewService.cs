using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models.EntityModels;
using Infrastructure.Models.Query;
using Infrastructure.Models.RequestModels;
using Infrastructure.Models.ResultModels;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Infrastructure.Services;

/// <summary>
/// Service that manages reviews and ratings.
/// </summary>
public class ReviewService(ReviewRepository reviewRepo, ReviewCrudService reviewCrudService, ProductRatingReviewRepository ratingReviewRepo,
    UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, HttpClient httpClient, IConfiguration config)
{
    private readonly ReviewRepository _reviewRepo = reviewRepo;
    private readonly ReviewCrudService _reviewCrudService = reviewCrudService;
    private readonly ProductRatingReviewRepository _ratingReviewRepo = ratingReviewRepo;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;

    #region Internal

    private static ProcessResult ProduceCatchError(Exception ex)
    {
        Debug.WriteLine(ex.Message);
        return new ProcessResult(StatusCodes.Status500InternalServerError, "ERROR: " + ex.Message);
    }

    /// <summary>
    /// Returns a user entity if valid and logged in.
    /// </summary>
    private async Task<UserEntity?> EnsureUserLoggedIn(ClaimsPrincipal userClaims)
    {
        var entity = await _userManager.GetUserAsync(userClaims);
        if (entity == null || !_signInManager.IsSignedIn(userClaims))
        {
            return null;
        }

        return entity;
    }

    /// <summary>
    /// Gets the ProductReviewRatingEntity for the specified product.
    /// If one doesn't exist (possibly if the topic trigger somehow fails),
    /// creates a new one in the database and returns that one instead.
    /// </summary>
    private async Task<ProductReviewRatingEntity> GetProductReviewRatingEntity(ProductModel product)
    {
        try
        {
            var entity = await _ratingReviewRepo.GetAsync(x =>  x.ProductId == product.Id, true);
            entity ??= await _ratingReviewRepo.CreateAsync(new ProductReviewRatingEntity { ProductId = product.Id! });
            
            return entity!;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        
        return null!;
    }

    /// <summary>
    /// Applies the average rating score of the product.
    /// </summary>
    private ProductReviewRatingEntity UpdateAverageProductRating(ProductReviewRatingEntity entity)
    {
        // First calculate average rating.
        decimal rating = 0;
        int reviewCount = 0;
        int ratingCount = 0;

        foreach (var userReview in entity.UserReviews)
        {
            if (userReview.Rating != null)
            {
                rating += userReview.Rating.Value;
                ratingCount++;
            }

            if (userReview.ReviewText != null)
            {
                reviewCount++;
            }
        }

        if (ratingCount > 0)
            rating /= (decimal)ratingCount;

        entity.AverageRating = rating;
        entity.ReviewCount = reviewCount;
        entity.RatingCount = ratingCount;

        return entity;
    }

    #endregion

    #region Review

    public async Task<ProcessResult<ReviewEntity>> ReviewProductAsync(ReviewRequestModel requestModel)
    {
        try
        {
            if (requestModel.Review == null || requestModel.Review.Rating == null 
                || requestModel.Review.ReviewText == null)
                return ProcessResult.ForbiddenResult("No rating or review was provided.")
                    .ToGeneric<ReviewEntity>();

            // Get user, we need both its ID and to ensure that they're logged in.
            var user = await EnsureUserLoggedIn(requestModel.UserClaims);
            if (user == null)
                return ProcessResult.ForbiddenResult("User must prove that they're logged in to leave a review.")
                    .ToGeneric<ReviewEntity>();

            string productId = requestModel.ProductId;

            // Ensure that product exists.
            var product = await _httpClient.GetFromJsonAsync<ProductModel>($"{_config["SingleProductUrl"]}/{productId}");
            
            if (product == null)
                return ProcessResult.NotFoundResult("The product being reviewed does not/no longer exists.").ToGeneric<ReviewEntity>();

            var reviewRatingEntity = await GetProductReviewRatingEntity(product);

            var review = reviewRatingEntity.UserReviews.FirstOrDefault(x => x.ProductId == productId);

            // If review already exists, update the entity instead.
            if (review != null)
            {
                review = await _reviewCrudService.UpdateAsync(review, requestModel.Review);
            }
            else
            {
                review = await _reviewCrudService.CreateAsync(requestModel.Review);
                reviewRatingEntity.UserReviews.Add(review);
            }

            if (review == null)
                return ProcessResult.InternalServerErrorResult("Something went wrong when trying to store the review.").ToGeneric<ReviewEntity>();

            await _ratingReviewRepo.UpdateAsync(UpdateAverageProductRating(reviewRatingEntity));

            return ProcessResult.CreatedResult("Review successfully stored.", review);
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<ReviewEntity>(); }
    }

    public async Task<ProcessResult<ReviewsResult>> GetAllReviewsOfProductAsync(ReviewsGetRequestModel qModel)
    {
        try
        {
            if (qModel.ProductId == null && qModel.ByUserId == null)
                return new ProcessResult<ReviewsResult>(StatusCodes.Status200OK, "ProductId and ByUserId are both null. Returning empty list.", new());

            var query = _reviewRepo.GetSet(false);

            if (qModel.ProductId != null)
            {
                query = query.Where(x => x.ProductId == qModel.ProductId);
            }

            if (qModel.ByUserId != null)
            {
                query = query.Where(x => x.UserId == qModel.ByUserId);
            }

            query = query.OrderByDescending(x => x.OriginallyPostedDate);

            var result = new ReviewsResult();

            int totalItemCount = await query.CountAsync();
            int totalPageCount = 0;

            // If for pagination, get the items for that page.
            if (qModel.PageQuery != null)
            {
                GetRequestPageQuery pageQ = qModel.PageQuery.Value;

                if (totalItemCount > 0)
                {
                    totalPageCount = (int)Math.Ceiling(totalItemCount / (decimal)pageQ.PageSize);

                    int takeCount = qModel.MaxItemCount != null
                        ? Math.Min(pageQ.PageSize, qModel.MaxItemCount.Value)
                        : pageQ.PageSize;

                    query = query.Skip((pageQ.PageNumber - 1) * pageQ.PageSize).Take(takeCount);
                }
            }
            else if (qModel.MaxItemCount != null)
            {
                query = query.Take(qModel.MaxItemCount.Value);
            }

            var list = await query.ToListAsync();

            var resultList = new HashSet<ReviewModel>();

            // Now add the reviews to the list. If we should include ,
            // ratings add those as well.
            foreach (var item in list)
            {
                var reviewModel = ReviewModelFactory.Create(item);
                resultList.Add(reviewModel);
            }

            result.TotalItemCount = totalItemCount;
            result.TotalPageCount = totalPageCount;
            result.Items = resultList;

            return new ProcessResult<ReviewsResult>(StatusCodes.Status200OK, "", result);
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<ReviewsResult>(); }
    }

    public async Task<ProcessResult<ReviewModel>> GetReviewAsync(ReviewGetRequestModel requestModel)
    {
        try
        {
            var entity = await _reviewRepo.GetAsync(x => x.ProductId == requestModel.ProductId && x.UserId == requestModel.UserId, requestModel.IncludeReview);
            if (entity == null)
                return ProcessResult.NotFoundResult("Could not find a review of specified product by specified user.").ToGeneric<ReviewModel>();

            return ProcessResult.OKResult("", ReviewModelFactory.Create(entity));
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<ReviewModel>(); }
    }

    public async Task<ProcessResult> DeleteReviewAndOrRatingAsync(ReviewDeleteRequestModel requestModel)
    {
        try
        {
            var entity = await _reviewRepo.GetAsync(x => x.ProductId == requestModel.ProductId && x.UserId == requestModel.UserId, true);
            if (entity == null)
                return ProcessResult.NotFoundResult("Could not find a review of specified product by specified user.").ToGeneric<ReviewModel>();

            if (requestModel.DeleteRating)
            {
                entity.Rating = null;
            }

            if (requestModel.DeleteReview)
            {
                var review = entity.ReviewText;
                entity.ReviewText = null;
                if (review != null)
                    await _reviewCrudService.DeleteTextAsync(review);
            }

            if (entity.Rating == null && entity.ReviewText == null)
            {
                await _reviewRepo.DeleteAsync(entity);
                return ProcessResult.OKResult("Rating and review text are gone so the entity has been deleted.");
            }

            return ProcessResult.OKResult("Successful partial deletion.");
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<ReviewModel>(); }
    }

    #endregion
}
