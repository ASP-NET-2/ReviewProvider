using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models;

public class BaseFeedbackItemResult<TModel> where TModel : class
{
    public int TotalItemCount { get; set; }
    public int TotalPageCount { get; set; }
    public IEnumerable<TModel> Items { get; set; } = new List<TModel>();
}
