namespace MRA.Pages.Domain.Entities;

public class Content: BaseEntity
{
    public string HtmlContent { get; set; } = "";
    public required string Lang { get; set; }
    public Guid PageId { get; set; }
    public Page? Page { get; set; }
}