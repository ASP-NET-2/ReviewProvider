using Infrastructure.Entities;
using System.Security.Claims;

namespace Infrastructure.Models.RequestModels;

public class ReviewCreationRequestModel
{
    public string ProductId { get; set; } = null!;
    public ClaimsPrincipal UserClaims { get; set; } = null!;
    public string ReviewTitle { get; set; } = null!;
    public string ReviewText { get; set; } = null!;
}
