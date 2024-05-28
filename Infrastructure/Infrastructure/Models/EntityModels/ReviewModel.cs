namespace Infrastructure.Models.EntityModels;

public class ReviewModel
{
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public ReviewTextModel? ReviewText { get; set; }
    public decimal? Rating { get; set; }
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}

public class ReviewTextModel
{
    public string ReviewTitle { get; set; } = null!;
    public string ReviewText { get; set; } = null!;
}