using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ReviewEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(120)] public string ReviewTitle { get; set; } = null!;
    [MaxLength(5000)] public string ReviewText { get; set; } = null!;
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}