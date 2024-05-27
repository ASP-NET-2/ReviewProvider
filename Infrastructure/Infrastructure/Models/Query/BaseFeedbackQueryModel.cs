namespace Infrastructure.Models.Query;

public class BaseFeedbackQueryModel
{
    public string? ProductId { get; set; }
    public string? ByUserId { get; set; }
    public PageQuery? PageQuery { get; set; }
    public int? MaxItemCount { get; set; }
}
