using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models;

public class ReviewModel : BaseFeedbackItemModel
{
    public string ReviewTitle { get; set; } = null!;
    public string ReviewText { get; set; } = null!;
}
