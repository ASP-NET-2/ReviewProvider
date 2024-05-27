using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models;

public class BaseFeedbackItemModel
{
    public string ProductId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime OriginallyPostedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;
}
