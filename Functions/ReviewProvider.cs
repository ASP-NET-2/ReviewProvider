using Infrastructure.Models.RequestModels;
using Infrastructure.Models.ResultModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResetPassword.Factories;

namespace ReviewProvider.Functions;

public class ReviewProvider(ILogger<ReviewProvider> logger, ReviewService reviewService)
{
    private readonly ILogger<ReviewProvider> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("ReviewProduct")]
    public async Task<IActionResult> ReviewProduct([HttpTrigger(AuthorizationLevel.Function, "post", "put")] HttpRequest req)
    {
        try
        {
            var requestModel = await req.ReadFromJsonAsync<ReviewRequestModel>();
            if (requestModel == null)
            {
                return new BadRequestObjectResult("Request model read as null.");
            }

            var result = await _reviewService.ReviewProductAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }

    [Function("GetAllReviews")]
    public async Task<IActionResult> GetAllReviews([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var requestModel = await req.ReadFromJsonAsync<ReviewsGetRequestModel>();
            if (requestModel == null)
            {
                return new BadRequestObjectResult("Request model read as null.");
            }

            var result = await _reviewService.GetAllReviewsOfProductAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }

    [Function("GetReview")]
    public async Task<IActionResult> GetReview([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var requestModel = await req.ReadFromJsonAsync<ReviewGetRequestModel>();
            if (requestModel == null)
            {
                return new BadRequestObjectResult("Request model read as null.");
            }

            var result = await _reviewService.GetReviewAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }

    [Function("DeleteReviewAndOrRating")]
    public async Task<IActionResult> DeleteReviewAndOrRating([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
    {
        try
        {
            var requestModel = await req.ReadFromJsonAsync<ReviewDeleteRequestModel>();
            if (requestModel == null)
            {
                return new BadRequestObjectResult("Request model read as null.");
            }

            var result = await _reviewService.DeleteReviewAndOrRatingAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}
