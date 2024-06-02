using Infrastructure.Models.RequestModels;
using Infrastructure.Models.ResultModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResetPassword.Factories;

namespace ReviewProvider.Functions;

public class GetProductFeedbackFunction(ILogger<GetProductFeedbackFunction> logger, ReviewService reviewService)
{
    private readonly ILogger<GetProductFeedbackFunction> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("GetProductFeedback")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetProductFeedback/{productId}")] HttpRequest req, string productId)
    {
        try
        {
            _logger.LogInformation("I try to start");
            var result = await _reviewService.GetProductFeedback(productId);
            if (!result.IsSuccessful)
            {
                _logger.LogError("No success. Reason: {msg}", result.Message);
            }

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}
