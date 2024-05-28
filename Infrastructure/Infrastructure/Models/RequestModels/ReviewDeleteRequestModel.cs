using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.RequestModels;

public class ReviewDeleteRequestModel
{
    public string ProductId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public bool DeleteRating { get; set; }
    public bool DeleteReview { get; set; }
}
