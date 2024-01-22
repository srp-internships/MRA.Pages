using MRA.Pages.Application.Contract.Page.Responses;

namespace MRA.Pages.Application.Contract.Page.Queries;

public class GetPagesQuery : IRequest<List<PageResponse>>
{
    public string? Application { get; set; }
    public bool? ShowInMenu { get; set; }
}