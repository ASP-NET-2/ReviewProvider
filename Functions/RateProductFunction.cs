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


public class RateProductFunction(ILogger<RateProductFunction> logger, ReviewService reviewService)
{
    private readonly ILogger<RateProductFunction> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("RateProduct")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", "put")] HttpRequest req)
    {
        try
        {
            var requestModel = await req.ReadFromJsonAsync<RatingRequestModel>();
            if (requestModel == null)
            {
                return new BadRequestObjectResult("Request model read as null.");
            }

            var result = await _reviewService.RateProductAsync(requestModel);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}