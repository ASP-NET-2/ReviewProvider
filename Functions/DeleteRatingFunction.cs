using Infrastructure.Models.RequestModels;
using Infrastructure.Models.ResultModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ResetPassword.Factories;

namespace ReviewProvider.Functions;

public class DeleteRatingFunction(ILogger<DeleteRatingFunction> logger, ReviewService reviewService)
{
    private readonly ILogger<DeleteRatingFunction> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("DeleteRating")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req)
    {
        try
        {
            var requestModel = new FeedbackDeleteRequestModel();
            requestModel.UserId = req.Query["UserId"]!;
            requestModel.ProductId = req.Query["ProductId"]!;

            if (requestModel.ProductId == null || requestModel.UserId == null)
                return ObjectResultFactory.CreateFromProcessResult(ProcessResult.BadRequestResult("Neither UserId or ProductId can be null."));

            var result = await _reviewService.DeleteRatingAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}