using MRA.Pages.Application.Contract.Page.Responses;

namespace MRA.Pages.Application.Contract.Page.Queries;

public class GetPages : IRequest<List<PageResponse>>
{
    public string? Application { get; set; }
    public bool? ShowInMenu { get; set; }
}