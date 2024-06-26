﻿using Infrastructure.Models.RequestModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ResetPassword.Factories;

namespace ReviewProvider.Functions;

public class GetUserFeedbackFunction(ILogger<GetUserFeedbackFunction> logger, ReviewService reviewService)
{
    private readonly ILogger<GetUserFeedbackFunction> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("GetUserFeedback")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            //var requestModel = await req.ReadFromJsonAsync<ReviewGetRequestModel>();
            //if (requestModel == null)
            //{
            //    return new BadRequestObjectResult("Request model read as null.");
            //}
            var model = new ReviewGetRequestModel();
            model.UserId = req.Query["UserId"];
            model.ProductId = req.Query["ProductId"];

            var result = await _reviewService.GetUserFeedbackAsync(model);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}
