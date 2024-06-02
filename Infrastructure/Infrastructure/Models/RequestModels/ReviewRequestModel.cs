using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System.Security.Claims;

namespace Infrastructure.Models.RequestModels;

public class ReviewRequestModel
{
    public string UserId { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public string ReviewTitle { get; set; } = null!;
    public string ReviewText { get; set; } = null!;
}
