using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System.Security.Claims;

namespace Infrastructure.Models.RequestModels;

public class ReviewRequestModel
{
    public ClaimsPrincipal UserClaims { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public ReviewModel Review { get; set; } = null!;
}
