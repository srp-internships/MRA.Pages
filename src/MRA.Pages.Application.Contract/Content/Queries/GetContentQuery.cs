namespace MRA.Pages.Application.Contract.Content.Queries;

public class GetContentQuery
{
    public required string PageName { get; set; }
    public required string Lang { get; set; }
}