using Infrastructure.Models.Query;

namespace Infrastructure.Models.RequestModels;

public class UserFeedbacksGetRequestModel
{
    public string? ProductId { get; set; }
    public string? ByUserId { get; set; }
    public GetRequestPageQuery? PageQuery { get; set; }
    public bool IncludeReviews { get; set; }
    public bool IncludeRatings { get; set; }
    public int? MaxItemCount { get; set; }
}
