namespace Infrastructure.Models.EntityModels;

public class UserFeedbackModel
{
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public ReviewModel? Review { get; set; }
    public decimal? Rating { get; set; }
}
