using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class ReviewTextEntity
{
    [Key] public string ReviewEntityId { get; set; } = null!;
    [MaxLength(120)] public string ReviewTitle { get; set; } = null!;
    [MaxLength(5000)] public string ReviewText { get; set; } = null!;
}