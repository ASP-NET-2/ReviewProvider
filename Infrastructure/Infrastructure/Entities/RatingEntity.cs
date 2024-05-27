using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public class RatingEntity : BaseFeedbackItemEntity
{

    [Column(TypeName = "decimal(5,2)")]
    public decimal Rating { get; set; }
}
