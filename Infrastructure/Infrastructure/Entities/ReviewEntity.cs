using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ReviewEntity : BaseFeedbackItemEntity
{
    [MaxLength(120)]
    public string ReviewTitle { get; set; } = null!;

    [MaxLength(5000)]
    public string ReviewText { get; set; } = null!;
}
