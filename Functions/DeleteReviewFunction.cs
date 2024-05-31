using Infrastructure.Models.RequestModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ResetPassword.Factories;

namespace ReviewProvider.Functions;

public class DeleteReviewFunction(ILogger<DeleteReviewFunction> logger, ReviewService reviewService)
{
    private readonly ILogger<DeleteReviewFunction> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("DeleteReview")]
    public async Task<IActionResult> DeleteReview([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
    {
        try
        {
            var requestModel = await req.ReadFromJsonAsync<FeedbackDeleteRequestModel>();
            if (requestModel == null)
            {
                return new BadRequestObjectResult("Request model read as null.");
            }

            var result = await _reviewService.DeleteReviewAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}
