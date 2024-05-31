﻿using Infrastructure.Entities;
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
public class ReviewService(FeedbackActionsService feedbackActionsService,
    UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, HttpClient httpClient, IConfiguration config)
{
    private readonly FeedbackActionsService _feedbackActionsService = feedbackActionsService;
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

    #endregion

    public async Task<ProcessResult> ReviewProductAsync(ReviewRequestModel requestModel)
    {
        try
        {
            if (requestModel.ReviewText == null || requestModel.ReviewTitle == null)
                return ProcessResult.ForbiddenResult("Empty reviews are not allowed.");

            // Get user, we need both its ID and to ensure that they're logged in.
            var user = await EnsureUserLoggedIn(requestModel.UserClaims);

            if (user == null)
            {
                return ProcessResult.ForbiddenResult("User must prove that they're logged in to leave a review.");
            }

            string productId = requestModel.ProductId;

            // Ensure that product exists.
            var product = await _httpClient.GetFromJsonAsync<ProductModel>($"{_config["SingleProductUrl"]}/{productId}");
            if (product == null)
            {
                return ProcessResult.NotFoundResult("The product being reviewed could not be found. The server could not be contacted, or the product does not/no longer exists.");
            }

            var reviewModel = new ReviewModel
            {
                ReviewTitle = requestModel.ReviewTitle,
                ReviewText = requestModel.ReviewText,
            };
            await _feedbackActionsService.ReviewProductAsync(productId, user.Id, reviewModel);

            return ProcessResult.CreatedResult("Review successfully stored.");
        }
        catch (Exception ex) { return ProduceCatchError(ex); }
    }

    public async Task<ProcessResult> RateProductAsync(RatingRequestModel requestModel)
    {
        try
        {
            // Get user, we need both its ID and to ensure that they're logged in.
            var user = await EnsureUserLoggedIn(requestModel.UserClaims);

            if (user == null)
            {
                return ProcessResult.ForbiddenResult("User must prove that they're logged in to leave a review.");
            }

            string productId = requestModel.ProductId;

            // Ensure that product exists.
            var product = await _httpClient.GetFromJsonAsync<ProductModel>($"{_config["SingleProductUrl"]}/{productId}");
            if (product == null)
            {
                return ProcessResult.NotFoundResult("The product being reviewed could not be found. The server could not be contacted, or the product does not/no longer exists.")
                    .ToGeneric<UserFeedbackEntity>();
            }

            var ratingModel = new RatingModel
            {
                Rating = requestModel.Rating.Rating,
            };
            await _feedbackActionsService.RateProductAsync(productId, user.Id, ratingModel);

            return ProcessResult.CreatedResult("Review successfully stored.");
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<UserFeedbackEntity>(); }
    }

    public async Task<ProcessResult<UserFeedbacksResult>> GetAllUserFeedbacksOfProductAsync(UserFeedbacksGetRequestModel qModel)
    {
        try
        {
            if (qModel.ProductId == null && qModel.ByUserId == null)
                return new ProcessResult<UserFeedbacksResult>(StatusCodes.Status200OK, "ProductId and ByUserId are both null. Returning empty list.", new());

            var result = new UserFeedbacksResult();

            int totalItemCount = await _feedbackActionsService.GetFeedbackInfoCountAsync(qModel.ProductId, qModel.ByUserId);
            int totalPageCount = 0;

            // If for pagination, get the items for that page.
            int startIndex = 0;
            int? takeCount = qModel.MaxItemCount;
            if (qModel.PageQuery != null)
            {
                GetRequestPageQuery pageQ = qModel.PageQuery.Value;

                if (totalItemCount > 0)
                {
                    totalPageCount = (int)Math.Ceiling(totalItemCount / (decimal)pageQ.PageSize);

                    takeCount = qModel.MaxItemCount != null
                        ? Math.Min(pageQ.PageSize, qModel.MaxItemCount.Value)
                        : pageQ.PageSize;

                    startIndex = pageQ.PageSize * (pageQ.PageNumber - 1);
                }
            }

            var list = await _feedbackActionsService.GetUserFeedbacksAsync(qModel.ProductId, qModel.ByUserId, 
                startIndex, takeCount, qModel.IncludeReviews, qModel.IncludeRatings);

            result.TotalItemCount = totalItemCount;
            result.TotalPageCount = totalPageCount;
            result.UserFeedbacks = list;

            return new ProcessResult<UserFeedbacksResult>(StatusCodes.Status200OK, "", result);
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<UserFeedbacksResult>(); }
    }

    public async Task<ProcessResult<UserFeedbackModel?>> GetUserFeedbackAsync(ReviewGetRequestModel requestModel)
    {
        try
        {
            var model = await _feedbackActionsService.GetUserFeedbackAsync(requestModel.ProductId, requestModel.UserId);

            return ProcessResult.OKResult("", model);
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<UserFeedbackModel?>(); }
    }

    public async Task<ProcessResult> DeleteReviewAsync(FeedbackDeleteRequestModel requestModel)
    {
        try
        {
            bool successful = await _feedbackActionsService.DeleteReviewAsync(requestModel.ProductId, requestModel.UserId);

            if (successful)
            {
                return ProcessResult.OKResult("Successful deletion.");
            }

            return ProcessResult.InternalServerErrorResult("An error occurred while trying to delete.");
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<UserFeedbackModel>(); }
    }

    public async Task<ProcessResult> DeleteRatingAsync(FeedbackDeleteRequestModel requestModel)
    {
        try
        {
            bool successful = await _feedbackActionsService.DeleteRatingAsync(requestModel.ProductId, requestModel.UserId);

            if (successful)
            {
                return ProcessResult.OKResult("Successful deletion.");
            }

            return ProcessResult.InternalServerErrorResult("An error occurred while trying to delete.");
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<UserFeedbackModel>(); }
    }

    public async Task<ProcessResult<ProductFeedbackInfoResult>> GetProductFeedback(string productId)
    {
        try
        {
            var result = await _feedbackActionsService.GetFeedbackInfoAsync(productId);

            if (result != null)
            {
                return ProcessResult.OKResult("", result);
            }

            return ProcessResult.InternalServerErrorResult("An error occurred while trying to delete.")
                .ToGeneric<ProductFeedbackInfoResult>();
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<ProductFeedbackInfoResult>(); }
    }
}
