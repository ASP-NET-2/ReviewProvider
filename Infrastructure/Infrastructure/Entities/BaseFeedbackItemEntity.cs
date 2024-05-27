namespace Infrastructure.Entities;

public abstract class BaseFeedbackItemEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ProductId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}
