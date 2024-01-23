using MRA.Pages.Application.Contract.Page.Responses;

namespace MRA.Pages.Application.Contract.Page.Queries;

public class GetPageQuery : IRequest<PageResponse>
{
    public required string Name { get; set; }
}