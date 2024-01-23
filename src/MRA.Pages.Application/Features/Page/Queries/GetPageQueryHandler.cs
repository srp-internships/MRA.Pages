using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Page.Queries;
using MRA.Pages.Application.Contract.Page.Responses;

namespace MRA.Pages.Application.Features.Page.Queries;

public class GetPageQueryHandler(IApplicationDbContext context, ICurrentUserService userService, IMapper mapper)
    : IRequestHandler<GetPageQuery, PageResponse>
{
    public async Task<PageResponse> Handle(GetPageQuery request, CancellationToken cancellationToken)
    {
        var firstOrDefault = await context.Pages.FirstOrDefaultAsync(s => s.Name == request.Name, cancellationToken);
        if (firstOrDefault == null || (!userService.IsSuperAdmin() && firstOrDefault.Disabled))
        {
            throw new ForbiddenAccessException();
        }

        return mapper.Map<PageResponse>(firstOrDefault);
    }
}