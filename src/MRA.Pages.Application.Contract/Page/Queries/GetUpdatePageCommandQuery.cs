using MRA.Pages.Application.Contract.Page.Commands;

namespace MRA.Pages.Application.Contract.Page.Queries;

public class GetUpdatePageCommandQuery : IRequest<UpdatePageCommand>
{
    public required string Name { get; set; }
}