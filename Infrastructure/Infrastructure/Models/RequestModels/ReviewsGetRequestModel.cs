using Infrastructure.Models.Query;

namespace Infrastructure.Models.RequestModels;

public class ReviewsGetRequestModel
{
    public string? ProductId { get; set; }
    public string? ByUserId { get; set; }
    public GetRequestPageQuery? PageQuery { get; set; }
    public int? MaxItemCount { get; set; }
}
