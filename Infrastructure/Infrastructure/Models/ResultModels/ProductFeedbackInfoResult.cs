using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.ResultModels;

public class ProductFeedbackInfoResult
{
    public decimal AverageRating { get; set; }
    public int RatingCount { get; set; }
    public int ReviewCount { get; set; }
}
