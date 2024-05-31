using Infrastructure.Models.RequestModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ResetPassword.Factories;

namespace ReviewProvider.Functions;

public class GetAllReviewsFunction(ILogger<GetAllReviewsFunction> logger, ReviewService reviewService)
{
    private readonly ILogger<GetAllReviewsFunction> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("GetAllReviews")]
    public async Task<IActionResult> GetAllReviews([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var requestModel = await req.ReadFromJsonAsync<UserFeedbacksGetRequestModel>();
            if (requestModel == null)
            {
                return new BadRequestObjectResult("Request model read as null.");
            }

            var result = await _reviewService.GetAllUserFeedbacksOfProductAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}
