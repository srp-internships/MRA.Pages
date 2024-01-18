namespace MRA.Pages.Application.Contract.Content.Commands;

public class DeleteContentCommand : IRequest<Unit>
{
    public required string Lang { get; set; }
    public required string PageName { get; set; }
}