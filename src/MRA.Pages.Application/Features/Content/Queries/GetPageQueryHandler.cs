using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Content.Queries;
using MRA.Pages.Application.Contract.Content.Responses;

namespace MRA.Pages.Application.Features.Content.Queries;

public class GetPageQueryHandler(IApplicationDbContext context, IMapper mapper, ICurrentUserService userService)
    : IRequestHandler<GetContentQuery, ContentResponse>
{
    public async Task<ContentResponse> Handle(GetContentQuery request, CancellationToken cancellationToken)
    {
        var content = await context.Contents.Include(p => p.Page)
            .FirstOrDefaultAsync(
                s => (s.Page != null ? s.Page.Name : null) == request.PageName && s.Lang == request.Lang,
                cancellationToken);
        if (content?.Page == null)
        {
            throw new NotFoundException(
                $"the content with pageName {request.PageName} and with language {request.Lang} not found");
        }

        if (string.IsNullOrEmpty(content.Page.Role) ||
            userService.IsSuperAdmin() ||
            userService.IsInRole(content.Page.Role))
        {
            return mapper.Map<ContentResponse>(content);
        }

        throw new ForbiddenAccessException();
    }
}