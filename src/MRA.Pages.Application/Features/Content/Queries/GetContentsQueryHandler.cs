using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Content.Queries;
using MRA.Pages.Application.Contract.Content.Responses;

namespace MRA.Pages.Application.Features.Content.Queries;

public class GetContentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetContentsQuery, List<ContentResponse>>
{
    public async Task<List<ContentResponse>> Handle(GetContentsQuery request, CancellationToken cancellationToken)
    {
        var page = await context.Pages.Include(s => s.Contents)
            .FirstOrDefaultAsync(s => s.Name == request.PageName, cancellationToken);
        if (page == null)
        {
            throw new NotFoundException($"page with name {request.PageName} not found");
        }

        return (page.Contents ?? new List<Domain.Entities.Content>()).Select(mapper.Map<ContentResponse>).ToList();
    }
}