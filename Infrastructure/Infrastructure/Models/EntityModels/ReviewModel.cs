namespace Infrastructure.Models.EntityModels;

public class ReviewModel
{
    public string ReviewTitle { get; set; } = null!;
    public string ReviewText { get; set; } = null!;
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}