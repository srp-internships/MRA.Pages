using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        if (request.Application != null)
        {
            query = query.Where(s => s.Application == request.Application);
        }

        if (request.ShowInMenu != null)
        {
            query = query.Where(s => s.ShowInMenu == request.ShowInMenu);
        }

        if (!userService.IsSuperAdmin())
        {
            query = query.Where(s => !s.Disabled);
        }

        var result = await query.ToArrayAsync(cancellationToken);
        if (!userService.IsSuperAdmin())
        {
            result = result.Where(s => s.Role == null || userService.IsInRole(s.Role.Split(',')))
                .ToArray(); //after lazy loading because in Where we cant call external methods

            var pageResponses = result.Select(mapper.Map<PageResponse>).ToList();
            foreach (var pageResponse in pageResponses)
            {
                var title = (await context.Contents.FirstOrDefaultAsync(s => s.Lang == request.Lang, cancellationToken))
                    ?.Title;
                if (title == null)
                {
                    pageResponses.Remove(pageResponse);
                }
                else
                {
                    pageResponse.Title = title;
                }
            }

            return pageResponses;
        }

        return result.Select(mapper.Map<PageResponse>).ToList();
    }
}