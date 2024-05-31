using Infrastructure.Entities;
using Infrastructure.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models.ResultModels;

public class UserFeedbacksResult
{
    public int TotalItemCount { get; set; }
    public int TotalPageCount { get; set; }
    public IEnumerable<UserFeedbackModel> UserFeedbacks { get; set; } = [];
}
