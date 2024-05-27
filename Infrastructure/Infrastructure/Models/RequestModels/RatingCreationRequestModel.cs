using Infrastructure.Entities;
using System.Security.Claims;

namespace Infrastructure.Models.RequestModels;

public class RatingCreationRequestModel
{
    public string ProductId { get; set; } = null!;
    public ClaimsPrincipal UserClaims { get; set; } = null!;
    public decimal Rating { get; set; }
}
