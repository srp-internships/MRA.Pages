namespace MRA.Pages.Domain.Entities;

public class Content
{
    public Guid Id { get; set; }
    public string HtmlContent { get; set; } = "";
    public required string Title { get; set; }
    public required string Lang { get; set; }
    public Guid PageId { get; set; }
    public Page? Page { get; set; }
}