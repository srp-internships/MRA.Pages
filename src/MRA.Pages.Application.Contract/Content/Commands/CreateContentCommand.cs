namespace MRA.Pages.Application.Contract.Content.Commands;

public class CreateContentCommand
{
    public required string PageName { get; set; }
    public string HtmlContent { get; set; } = "";
    public required string Title { get; set; }
    public required string Lang { get; set; }
}