using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Content.Commands;

namespace MRA.Pages.Application.Features.Content.Commands;

public class UpdateContentCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdateContentCommand, Unit>
{
    public async Task<Unit> Handle(UpdateContentCommand request, CancellationToken cancellationToken)
    {
        var pageId = await context.Pages.Where(s => s.Name == request.PageName).Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (pageId.Equals(Guid.Empty))
        {
            throw new NotFoundException($"page with name {request.PageName} not found");
        }

        var old = await context.Contents.FirstOrDefaultAsync(s => s.PageId == pageId && s.Lang == request.Lang,
            cancellationToken);
        if (old == null)
        {
            throw new NotFoundException($"the content with lang {request.OldLang} not found");
        }

        var content = mapper.Map(request, old);
        content.PageId = pageId;
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}