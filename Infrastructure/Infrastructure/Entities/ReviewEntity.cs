using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class ReviewEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    [Column(TypeName = "decimal(5,2)")]
    public RatingEntity? Rating { get; set; }
    public ReviewTextEntity? ReviewText { get; set; }
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}
