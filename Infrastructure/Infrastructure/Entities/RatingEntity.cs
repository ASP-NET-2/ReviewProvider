using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class RatingEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Precision(18, 4)] public decimal Rating { get; set; }
}
