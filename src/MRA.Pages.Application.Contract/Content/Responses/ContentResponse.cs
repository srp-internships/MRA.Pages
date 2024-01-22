namespace MRA.Pages.Application.Contract.Content.Responses;

public sealed record ContentResponse
{
    public required string Lang { get; set; }
    public required string PageName { get; set; }
    public string HtmlContent { get; set; } = "";
    public required string Title { get; set; }
}