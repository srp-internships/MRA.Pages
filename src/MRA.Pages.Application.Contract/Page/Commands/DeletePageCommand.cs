namespace MRA.Pages.Application.Contract.Page.Commands;

public class DeletePageCommand : IRequest<Unit>
{
    public required string Name { get; set; }
}