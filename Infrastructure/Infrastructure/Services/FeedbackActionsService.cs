﻿using Infrastructure.Data.Contexts;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models.EntityModels;
using Infrastructure.Models.RequestModels;
using Infrastructure.Models.ResultModels;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class FeedbackActionsService(UserFeedbackRepository userFeedbackRepo, ReviewRepository reviewRepo, 
    RatingRepository ratingRepo, ProductFeedbackRepository feedbackRepo, FeedbackItemsDataContext feedbackItemsDataContext)
{
    private readonly UserFeedbackRepository _userFeedbackRepo = userFeedbackRepo;
    private readonly ReviewRepository _reviewRepo = reviewRepo;
    private readonly RatingRepository _ratingRepo = ratingRepo;
    private readonly ProductFeedbackRepository _productFeedbackRepo = feedbackRepo;
    private readonly FeedbackItemsDataContext _feedbackItemsDataContext = feedbackItemsDataContext;

    #region Internal

    private async Task<ProductFeedbackEntity> GetOrCreateProductFeedbackEntity(string productId, bool saveChangesIfCreate = true)
    {
        try
        {
            var entity = await _productFeedbackRepo.GetAsync(x => x.ProductId == productId, true);
            entity ??= await _productFeedbackRepo.CreateAsync(new ProductFeedbackEntity { ProductId = productId }, saveChangesIfCreate);

            return entity;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return null!;
    }

    private async Task<UserFeedbackEntity> GetOrCreateUserFeedbackEntity(string productId, string userId, bool saveChangesIfCreate = true)
    {
        try
        {
            var feedbackEntity = await GetOrCreateProductFeedbackEntity(productId);

            var userFeedback = feedbackEntity.UserFeedbacks.FirstOrDefault(x => x.UserId == userId);
            
            if (userFeedback == null) // Create an entity if it doesn't already exist.
            {
                var newEntity = new UserFeedbackEntity { ProductId = productId, UserId = productId };
                userFeedback = await _userFeedbackRepo.CreateAsync(newEntity, false);
                
                feedbackEntity.UserFeedbacks.Add(userFeedback);
                feedbackEntity = await _productFeedbackRepo.UpdateAsync(feedbackEntity, saveChangesIfCreate);
            }
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
        bool result = feedbackEntity.Review == null && feedbackEntity.Rating == null;
        if (result)
        {
            await _userFeedbackRepo.DeleteAsync(feedbackEntity);
            var productFeedback = await GetOrCreateProductFeedbackEntity(feedbackEntity.ProductId);
            await _productFeedbackRepo.UpdateAsync(UpdateAverageProductRating(productFeedback), false);
        }

        return result;
    }

    #endregion

    public async Task<bool> ReviewProductAsync(string productId, string userId, ReviewModel model)
    {
        try
        {
            var userFeedbackEntity = await GetOrCreateUserFeedbackEntity(productId, userId, false);

            if (userFeedbackEntity.Review == null)
            {
                var newEntity = ReviewTextEntityFactory.Create(model);
                userFeedbackEntity.Review = await _reviewRepo.CreateAsync(newEntity, false);
            }
            else
            {
                var updateEntity = ReviewTextEntityFactory.Update(userFeedbackEntity.Review, model);
                userFeedbackEntity.Review = await _reviewRepo.UpdateAsync(updateEntity, false);
            }

            await _userFeedbackRepo.UpdateAsync(userFeedbackEntity, false);

            var productFeedback = await GetOrCreateProductFeedbackEntity(productId);
            await _productFeedbackRepo.UpdateAsync(UpdateAverageProductRating(productFeedback));

            await _feedbackItemsDataContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }

    public async Task<bool> RateProductAsync(string productId, string userId, RatingModel model)
    {
        try
        {
            var userFeedbackEntity = await GetOrCreateUserFeedbackEntity(productId, userId, false);

            if (userFeedbackEntity.Rating == null)
            {
                var newEntity = RatingEntityFactory.Create(model);

                userFeedbackEntity.Rating = await _ratingRepo.CreateAsync(newEntity, false);
            }
            else
            {
                var updateEntity = RatingEntityFactory.Update(userFeedbackEntity.Rating, model);
                userFeedbackEntity.Rating = await _ratingRepo.UpdateAsync(updateEntity, false);
            }

            await _userFeedbackRepo.UpdateAsync(userFeedbackEntity, false);

            var productFeedback = await GetOrCreateProductFeedbackEntity(productId);
            await _productFeedbackRepo.UpdateAsync(UpdateAverageProductRating(productFeedback));

            await _feedbackItemsDataContext.SaveChangesAsync();

            return true;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }

    public async Task<UserFeedbackModel?> GetUserFeedbackAsync(string? productId, string? userId)
    {
        try
        {
            var entity = await _userFeedbackRepo.GetAsync(x => x.ProductId == productId && x.UserId == userId, true);
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
            if (productId == null && userId == null)
                return [];

            var query = _userFeedbackRepo.GetSet(false);

            if (includeReviews) query = query.Include(x => x.Review);
            if (includeRatings) query = query.Include(x => x.Rating);

            if (startIndex != null) query = query.Skip(startIndex.Value);
            if (takeCount != null)  query = query.Take(takeCount.Value);

            var list = await query.ToListAsync();

            var result = new List<UserFeedbackModel>();

            foreach (var item in list)
            {
                var model = UserFeedbackModelFactory.Create(item);
                result.Add(model);
            }

            result = (List<UserFeedbackModel>)result.OrderByDescending(x => x.Review != null 
            ? x.Review.OriginallyPostedDate 
            : DateTime.MinValue);

            return result;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return [];
    }

    public async Task<ProductFeedbackInfoResult> GetFeedbackInfoAsync(string productId)
    {
        try
        {
            var productEntity = await GetOrCreateProductFeedbackEntity(productId);
            
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
            if (productId == null &&  userId == null) 
                return 0;

            var query = _userFeedbackRepo.GetSet(false);

            if (productId != null) query = query.Where(x => x.ProductId == productId);
            if (userId != null) query = query.Where(query => query.UserId == userId);

            return await query.CountAsync();
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return 0;
    }

    public async Task<bool> DeleteReviewAsync(string productId, string userId)
    {
        try
        {
            if (productId == null && userId == null)
            {
                return false;
            }

            var feedbackRepo = await _userFeedbackRepo.GetAsync(x => x.ProductId == productId && x.UserId == userId);
            if (feedbackRepo == null)
                return false;

            var review = feedbackRepo.Review;
            if (review == null)
                return false;

            await _reviewRepo.DeleteAsync(review, false);

            feedbackRepo.Review = null;

            if (!await DeleteUserFeedbackIfEmpty(feedbackRepo))
            {
                await _userFeedbackRepo.UpdateAsync(feedbackRepo, false);
            }

            await _feedbackItemsDataContext.SaveChangesAsync();
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }

    public async Task<bool> DeleteRatingAsync(string productId, string userId)
    {
        try
        {
            if (productId == null && userId == null)
            {
                return false;
            }

            var userFeedback = await _userFeedbackRepo.GetAsync(x => x.ProductId == productId && x.UserId == userId);
            if (userFeedback == null)
                return false;

            var rating = userFeedback.Rating;
            if (rating == null)
                return false;

            await _ratingRepo.DeleteAsync(rating, false);

            userFeedback.Rating = null;

            if (!await DeleteUserFeedbackIfEmpty(userFeedback))
            {
                await _userFeedbackRepo.UpdateAsync(userFeedback, false);
            }

            await _feedbackItemsDataContext.SaveChangesAsync();
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }

        return false;
    }
}