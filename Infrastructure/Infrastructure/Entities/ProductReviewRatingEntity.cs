using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class ProductReviewRatingEntity
{
    [Key] 
    public string ProductId { get; set; } = null!;

    public ICollection<ReviewEntity> UserReviews { get; set; } = new HashSet<ReviewEntity>();

    [Column(TypeName = "decimal(5,2)")]
    public decimal AverageRating { get; set; }
    public int RatingCount { get; set; }
    public int ReviewCount { get; set; }
}
