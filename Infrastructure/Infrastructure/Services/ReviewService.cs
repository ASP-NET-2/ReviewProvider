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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Infrastructure.Services;

/// <summary>
/// Service that manages reviews and ratings.
/// </summary>
public class ReviewService(FeedbackActionsService feedbackActionsService,
    HttpClient httpClient, IConfiguration config, ILogger<ReviewService> logger)
{
    private readonly FeedbackActionsService _feedbackActionsService = feedbackActionsService;
    private readonly HttpClient _httpClient = httpClient;
    private readonly IConfiguration _config = config;
    private readonly ILogger<ReviewService> _logger = logger;
    

    #region Internal

    private static ProcessResult ProduceCatchError(Exception ex)
    {
        Debug.WriteLine(ex.Message);
        return new ProcessResult(StatusCodes.Status500InternalServerError, "ERROR: " + ex.Message);
    }

    #endregion

    public async Task<ProcessResult> ReviewProductAsync(ReviewRequestModel requestModel)
    {
        try
        {
            if (requestModel.ReviewText == null || requestModel.ReviewTitle == null)
                return ProcessResult.BadRequestResult("Empty reviews are not allowed.");

            // Ensure that user exists.
            var user = await _feedbackActionsService.GetUserAsync(requestModel.UserId);
            if (user == null)
            {
                return ProcessResult.NotFoundResult("The reviewing user could not be found. The server could not be contacted, or the user does not/no longer exists.");
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
            if (requestModel.Rating == null)
            {
                return ProcessResult.BadRequestResult("No rating was included in the request.")
                    .ToGeneric<UserFeedbackEntity>();
            }

            // Ensure that user exists.
            var user = await _feedbackActionsService.GetUserAsync(requestModel.UserId);
            if (user == null)
            {
                return ProcessResult.NotFoundResult("The rating user could not be found. The server could not be contacted, or the user does not/no longer exists.");
            }

            // Ensure that product exists.

            //var product = await _httpClient.GetFromJsonAsync<ProductModel>($"{_config["SingleProductUrl"]}/{requestModel.ProductId}");
            var response = await _httpClient.GetAsync($"{_config["SingleProductUrl"]}/{requestModel.ProductId}");
            _logger.LogInformation("URL: {msg}", $"{_config["SingleProductUrl"]}/{requestModel.ProductId}");
            string str = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("{msg}", str);

            var product = JsonConvert.DeserializeObject<ProductModel>(await response.Content.ReadAsStringAsync());
            if (product == null)
            {
                return ProcessResult.NotFoundResult("The product being reviewed could not be found. The server could not be contacted, or the product does not/no longer exists.");
            }

            await _feedbackActionsService.RateProductAsync(requestModel.ProductId, user.Id, requestModel.Rating);

            return ProcessResult.CreatedResult("Review successfully stored.");
        }
        catch (Exception ex) { return ProduceCatchError(ex); }
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

            return ProcessResult.InternalServerErrorResult("An error occurred while trying to get the feedback info result.")
                .ToGeneric<ProductFeedbackInfoResult>();
        }
        catch (Exception ex) { return ProduceCatchError(ex).ToGeneric<ProductFeedbackInfoResult>(); }
    }
}
