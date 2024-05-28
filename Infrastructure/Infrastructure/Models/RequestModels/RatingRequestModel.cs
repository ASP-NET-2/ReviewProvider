using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System.Security.Claims;

namespace Infrastructure.Models.RequestModels;

public class RatingRequestModel
{
    public ClaimsPrincipal UserClaims { get; set; } = null!;
    public string ProductId { get; set; } = null!;
    public RatingModel Rating { get; set; } = null!;
}
