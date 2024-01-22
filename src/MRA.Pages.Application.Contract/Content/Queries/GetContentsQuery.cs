using MRA.Pages.Application.Contract.Content.Responses;

namespace MRA.Pages.Application.Contract.Content.Queries;

public class GetContentsQuery : IRequest<List<ContentResponse>>
{
    public required string PageName { get; set; }
}