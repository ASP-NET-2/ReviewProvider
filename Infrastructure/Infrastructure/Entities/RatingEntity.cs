using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class RatingEntity
{
    [Key] public string ReviewEntityId { get; set; } = null!;
    public decimal Rating { get; set; }
}
