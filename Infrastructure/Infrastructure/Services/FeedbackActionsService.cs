using Infrastructure.Data.Contexts;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models.EntityModels;
using Infrastructure.Models.RequestModels;
using Infrastructure.Models.ResultModels;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class FeedbackActionsService(UserFeedbackRepository userFeedbackRepo, ReviewRepository reviewRepo, RatingRepository ratingRepo,
    ProductFeedbackRepository feedbackRepo, IDbContextFactory<FeedbackItemsDataContext> dbContextFactory, ILogger<FeedbackActionsService> logger, 
    IDbContextFactory<IdentityDataContext> identityContextFactory, UserRepository userRepository)
{
    private readonly UserFeedbackRepository _userFeedbackRepo = userFeedbackRepo;
    private readonly ReviewRepository _reviewRepo = reviewRepo;
    private readonly RatingRepository _ratingRepo = ratingRepo;
    private readonly ProductFeedbackRepository _productFeedbackRepo = feedbackRepo;
    private readonly IDbContextFactory<IdentityDataContext> _identityContextFactory = identityContextFactory;
    private readonly UserRepository _userRepository = userRepository;
    private readonly IDbContextFactory<FeedbackItemsDataContext> _feedbackContextFactory = dbContextFactory;
    private readonly ILogger<FeedbackActionsService> _logger = logger;

    #region Internal

    private async Task<ProductFeedbackEntity> GetOrCreateProductFeedbackEntityAsync(FeedbackItemsDataContext ctx, string productId, bool saveChangesIfCreate = true)
    {
        try
        {
            var entity = await _productFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId, true);
            entity ??= await _productFeedbackRepo.CreateAsync(ctx, new ProductFeedbackEntity { ProductId = productId }, saveChangesIfCreate);

            return entity;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    private async Task<UserFeedbackEntity> GetOrCreateUserFeedbackEntityAsync(FeedbackItemsDataContext ctx, string productId, string userId, bool saveChangesIfCreate = true)
    {
        try
        {
            var feedbackEntity = await GetOrCreateProductFeedbackEntityAsync(ctx, productId);

            var userFeedback = feedbackEntity.UserFeedbacks.FirstOrDefault(x => x.UserId == userId);

            if (userFeedback == null) // Create an entity if it doesn't already exist.
            {
                var newEntity = new UserFeedbackEntity { ProductId = productId, UserId = userId };
                userFeedback = await _userFeedbackRepo.CreateAsync(ctx, newEntity, saveChangesIfCreate);

                feedbackEntity.UserFeedbacks.Add(userFeedback);
                feedbackEntity = await _productFeedbackRepo.UpdateAsync(ctx, feedbackEntity, saveChangesIfCreate);
            }

            return userFeedback;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    private ProductFeedbackEntity UpdateAverageProductRating(ProductFeedbackEntity entity)
    {
        // First calculate average rating.
        decimal rating = 0;
        int reviewCount = 0;
        int ratingCount = 0;

        foreach (var userFeedback in entity.UserFeedbacks)
        {
            if (userFeedback.Rating != null)
            {
                rating += userFeedback.Rating.Rating;
                ratingCount++;
            }

            if (userFeedback.Review != null)
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

    private async Task<bool> DeleteUserFeedbackIfEmpty(UserFeedbackEntity feedbackEntity)
    {
        using var ctx = await _feedbackContextFactory.CreateDbContextAsync();
        bool result = feedbackEntity.Review == null && feedbackEntity.Rating == null;
        if (result)
        {
            await _userFeedbackRepo.DeleteAsync(ctx, feedbackEntity, true);
            var productFeedback = await GetOrCreateProductFeedbackEntityAsync(ctx, feedbackEntity.ProductId);
            await _productFeedbackRepo.UpdateAsync(ctx, UpdateAverageProductRating(productFeedback), true);
        }

        return result;
    }

    #endregion

    public async Task<UserEntity?> GetUserAsync(string userId)
    {
        try
        {
            using var ctx = await _identityContextFactory.CreateDbContextAsync();
            return await _userRepository.GetAsync(ctx, x => x.Id == userId);
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null;
    }

    public async Task<bool> ReviewProductAsync(string productId, string userId, ReviewModel model)
    {
        try
        {
            if (model == null || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId))
            {
                return false;
            }

            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();

            var userFeedbackEntity = await GetOrCreateUserFeedbackEntityAsync(ctx, productId, userId, true);

            if (userFeedbackEntity.Review == null)
            {
                var newEntity = ReviewTextEntityFactory.Create(model);
                userFeedbackEntity.Review = await _reviewRepo.CreateAsync(ctx, newEntity, false);
            }
            else
            {
                var updateEntity = ReviewTextEntityFactory.Update(userFeedbackEntity.Review, model);
                userFeedbackEntity.Review = await _reviewRepo.UpdateAsync(ctx, updateEntity, false);
            }

            await _userFeedbackRepo.UpdateAsync(ctx, userFeedbackEntity, false);

            var productFeedback = await GetOrCreateProductFeedbackEntityAsync(ctx, productId);
            await _productFeedbackRepo.UpdateAsync(ctx, UpdateAverageProductRating(productFeedback), false);

            int amountSaved = await ctx.SaveChangesAsync();
            _logger.LogInformation("Items saved: {amntSaved}", amountSaved);

            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }

    public async Task<bool> RateProductAsync(string productId, string userId, RatingModel model)
    {
        try
        {
            if (model == null || string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId))
            {
                return false;
            }

            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();
            var userFeedbackEntity = await GetOrCreateUserFeedbackEntityAsync(ctx, productId, userId, true);

            if (userFeedbackEntity.Rating == null)
            {
                var newEntity = RatingEntityFactory.Create(model);

                userFeedbackEntity.Rating = await _ratingRepo.CreateAsync(ctx, newEntity, false);
            }
            else
            {
                var updateEntity = RatingEntityFactory.Update(userFeedbackEntity.Rating, model);
                userFeedbackEntity.Rating = await _ratingRepo.UpdateAsync(ctx, updateEntity, false);
            }

            await _userFeedbackRepo.UpdateAsync(ctx, userFeedbackEntity, false);

            var productFeedback = await GetOrCreateProductFeedbackEntityAsync(ctx, productId);
            await _productFeedbackRepo.UpdateAsync(ctx, UpdateAverageProductRating(productFeedback), false);

            int amountSaved = await ctx.SaveChangesAsync();
            _logger.LogInformation("Items saved: {amntSaved}", amountSaved);
            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }

    public async Task<UserFeedbackModel?> GetUserFeedbackAsync(string? productId, string? userId)
    {
        try
        {
            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();
            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId))
                return null;

            var entity = await _userFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId && x.UserId == userId, true);
            if (entity == null)
                return null;

            var model = UserFeedbackModelFactory.Create(entity);
            return model;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    public async Task<IEnumerable<UserFeedbackModel>> GetUserFeedbacksAsync(string? productId, string? userId, int? startIndex, int? takeCount, bool includeReviews, bool includeRatings)
    {
        try
        {
            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();
            if (string.IsNullOrEmpty(productId) && string.IsNullOrEmpty(userId))
                return [];

            var query = _userFeedbackRepo.GetSet(ctx, false);

            if (!string.IsNullOrEmpty(productId)) query = query.Where(x => x.ProductId == productId);
            if (!string.IsNullOrEmpty(userId)) query = query.Where(x => x.UserId == userId);

            if (includeReviews) query = query.Include(x => x.Review);
            if (includeRatings) query = query.Include(x => x.Rating);

            query = query.Skip(startIndex ?? 0);
            if (takeCount != null) query = query.Take(takeCount.Value);

            var list = await query.ToListAsync();

            var result = new List<UserFeedbackModel>();

            foreach (var item in list)
            {
                var model = UserFeedbackModelFactory.Create(item);
                result.Add(model);
            }

            result = result.OrderByDescending(x => x.Review != null
            ? x.Review.OriginallyPostedDate
            : DateTime.MinValue).ToList();

            return result;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return [];
    }

    public async Task<ProductFeedbackInfoResult> GetFeedbackInfoAsync(string productId)
    {
        try
        {
            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();

            var productEntity = await GetOrCreateProductFeedbackEntityAsync(ctx, productId);

            var result = ProductFeedbackInfoResultFactory.Create(productEntity);

            return result;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    public async Task<int> GetFeedbackInfoCountAsync(string? productId, string? userId)
    {
        try
        {
            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();
            if (string.IsNullOrEmpty(productId) && string.IsNullOrEmpty(userId))
                return 0;

            var query = _userFeedbackRepo.GetSet(ctx, false);

            if (!string.IsNullOrEmpty(productId)) query = query.Where(x => x.ProductId == productId);
            if (!string.IsNullOrEmpty(userId)) query = query.Where(x => x.UserId == userId);

            return await query.CountAsync();
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return 0;
    }

    public async Task<bool> DeleteReviewAsync(string productId, string userId)
    {
        try
        {
            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();

            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var userFeedback = await _userFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId && x.UserId == userId, true);
            if (userFeedback == null)
                return false;

            var review = userFeedback.Review;
            if (review == null)
                return false;

            await _reviewRepo.DeleteAsync(ctx, review, true);

            userFeedback.Review = null;

            if (!await DeleteUserFeedbackIfEmpty(userFeedback))
            {
                await _userFeedbackRepo.UpdateAsync(ctx, userFeedback, false);

                var productFeedback = await GetOrCreateProductFeedbackEntityAsync(ctx, productId, false);
                await _productFeedbackRepo.UpdateAsync(ctx, UpdateAverageProductRating(productFeedback), false);
            }

            await ctx.SaveChangesAsync();

            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }

    public async Task<bool> DeleteRatingAsync(string productId, string userId)
    {
        try
        {
            using var ctx = await _feedbackContextFactory.CreateDbContextAsync();

            if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var userFeedback = await _userFeedbackRepo.GetAsync(ctx, x => x.ProductId == productId && x.UserId == userId, true);
            if (userFeedback == null)
                return false;

            var rating = userFeedback.Rating;
            if (rating == null)
                return false;

            await _ratingRepo.DeleteAsync(ctx, rating, true);

            userFeedback.Rating = null;

            if (!await DeleteUserFeedbackIfEmpty(userFeedback))
            {
                await _userFeedbackRepo.UpdateAsync(ctx, userFeedback, false);

                var productFeedback = await GetOrCreateProductFeedbackEntityAsync(ctx, productId, false);
                await _productFeedbackRepo.UpdateAsync(ctx, UpdateAverageProductRating(productFeedback), false);
            }

            await ctx.SaveChangesAsync();

            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }
}
