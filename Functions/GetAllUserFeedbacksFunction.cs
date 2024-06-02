using Infrastructure.Models.Query;
using Infrastructure.Models.RequestModels;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ResetPassword.Factories;

namespace ReviewProvider.Functions;

public class GetAllUserFeedbacksFunction(ILogger<GetAllUserFeedbacksFunction> logger, ReviewService reviewService)
{
    private readonly ILogger<GetAllUserFeedbacksFunction> _logger = logger;
    private readonly ReviewService _reviewService = reviewService;

    [Function("GetAllUserFeedbacks")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var model = new UserFeedbacksGetRequestModel();

        try
        {
            model.ProductId = req.Query["ProductId"];
            model.ByUserId = req.Query["ByUserId"];
            if (req.Query["HasPageQuery"] == "true")
            {
                model.PageQuery = new GetRequestPageQuery
                {
                    PageNumber = !string.IsNullOrEmpty(req.Query["PageNumber"])
                    ? int.Parse(req.Query["PageNumber"]!) : 1,

                    PageSize = !string.IsNullOrEmpty(req.Query["PageSize"])
                    ? int.Parse(req.Query["PageSize"]!) : 9,
                };
            }
            model.IncludeReviews = bool.Parse(req.Query["IncludeReviews"]!);
            model.IncludeRatings = bool.Parse(req.Query["IncludeRatings"]!);
            model.MaxItemCount = int.TryParse(req.Query["MaxItemCount"], out int itemCount) ? itemCount : null;
        }
        catch (Exception ex) { return new ObjectResult($"Failed to parse query request: {ex.Message}") { StatusCode = StatusCodes.Status400BadRequest }; }

        try
        {
            var result = await _reviewService.GetAllUserFeedbacksOfProductAsync(model);

            return ObjectResultFactory.CreateFromProcessResult(result);
        }
        catch (Exception ex) { return new ObjectResult(ex.Message) { StatusCode = 500 }; }
    }
}
