using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Page.Queries;
using MRA.Pages.Application.Contract.Page.Responses;

namespace MRA.Pages.Application.Features.Page.Queries;

public class GetPagesQueryHandler(IApplicationDbContext context, ICurrentUserService userService, IMapper mapper)
    : IRequestHandler<GetPagesQuery, List<PageResponse>>
{
    public async Task<List<PageResponse>> Handle(GetPagesQuery request, CancellationToken cancellationToken)
    {
        var query = context.Pages.Where(s => true);

        if (request.ShowInMenu != null)
        {
            query = query.Where(s => s.ShowInMenu == request.ShowInMenu);
        }

        if (!userService.IsSuperAdmin())
        {
            query = query.Where(s => !s.Disabled);
        }

        var result = await query.AsNoTracking().ToArrayAsync(cancellationToken);
        if (!userService.IsSuperAdmin())
        {
            if (string.IsNullOrEmpty(request.Lang))
            {
                throw new BadRequestException("You must choose language");
            }
            result = result.Where(s => s.Role == null || userService.IsInRole(s.Role.Split(',')))
                .ToArray(); //after lazy loading because in Where we cant call external methods

            var pageResponses = result.Select(mapper.Map<PageResponse>).ToList();
            result = null;
            
            for (var i = 0; i < pageResponses.Count; i++)
            {
                var pageResponse = pageResponses[i];
                var title = (await context.Contents.Include(f => f.Page)
                        .FirstOrDefaultAsync(s => s.Lang == request.Lang && s.Page.Name == pageResponse.Name,
                            cancellationToken))
                    ?.Title;
                if (title == null)
                {
                    pageResponses.Remove(pageResponse);
                }
                else
                {
                    pageResponse.Title = title;
                }

                if (title != null)
                {
                    if (!string.IsNullOrEmpty(pageResponse.Application))
                    {
                        if (string.IsNullOrEmpty(request.Application) ||
                            !pageResponse.Application.Split(',').Contains(request.Application))
                        {
                            pageResponses.Remove(pageResponse);
                        }
                    }
                }
            }

            return pageResponses;
        }

        return result.Select(mapper.Map<PageResponse>).ToList();
    }
}