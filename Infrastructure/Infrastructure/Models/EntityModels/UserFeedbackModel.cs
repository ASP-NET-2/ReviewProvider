using Infrastructure.Entities;

namespace Infrastructure.Models.EntityModels;

public class UserFeedbackModel
{
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public ReviewModel? Review { get; set; }
    public RatingModel? Rating { get; set; }
}
