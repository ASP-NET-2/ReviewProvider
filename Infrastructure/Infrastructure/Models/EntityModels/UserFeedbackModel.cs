namespace Infrastructure.Models.EntityModels;

public class UserFeedbackModel
{
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public ReviewModel? Review { get; set; }
    public decimal? Rating { get; set; }
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}

public class ReviewModel
{
    public string ReviewTitle { get; set; } = null!;
    public string ReviewText { get; set; } = null!;
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}