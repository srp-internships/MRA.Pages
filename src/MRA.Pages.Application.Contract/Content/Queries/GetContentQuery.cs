using MRA.Pages.Application.Contract.Content.Responses;

namespace MRA.Pages.Application.Contract.Content.Queries;

public class GetContentQuery : IRequest<ContentResponse>
{
    public required string PageName { get; set; }
    public required string Lang { get; set; }
}