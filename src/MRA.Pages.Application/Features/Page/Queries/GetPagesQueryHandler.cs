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
            var finalResult = new List<PageResponse>();
            result = null;

            foreach (var pageResponse in pageResponses)
            {
                var title = (await context.Contents.Include(f => f.Page)
                        .FirstOrDefaultAsync(s => s.Lang == request.Lang && s.Page.Name == pageResponse.Name,
                            cancellationToken))
                    ?.Title;

                if (title != null)
                {
                    pageResponse.Title = title;
                    if (!string.IsNullOrEmpty(pageResponse.Application))
                    {
                        if (string.IsNullOrEmpty(request.Application) ||
                            !pageResponse.Application.Split(',').Contains(request.Application))
                        {
                            finalResult.Remove(pageResponse);
                        }
                        else
                        {
                            finalResult.Add(pageResponse);
                        }
                    }
                    else
                    {
                        finalResult.Add(pageResponse);
                    }
                }
            }

            return finalResult;
        }

        return result.Select(mapper.Map<PageResponse>).ToList();
    }
}