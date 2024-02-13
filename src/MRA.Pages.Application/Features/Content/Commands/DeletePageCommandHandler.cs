using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Content.Commands;

namespace MRA.Pages.Application.Features.Content.Commands;

public class DeleteContentCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteContentCommand, Unit>
{
    public async Task<Unit> Handle(DeleteContentCommand request, CancellationToken cancellationToken)
    {
        var page = await context.Pages.FirstOrDefaultAsync(p => p.Name == request.PageName, cancellationToken);
        if (page == null)
        {
            throw new NotFoundException($"The page with name {request.PageName} does not exist");
        }

        var content =
            await context.Contents.FirstOrDefaultAsync(p => p.PageId == page.Id && p.Lang == request.Lang,
                cancellationToken);
        var contents = context.Contents.ToList();
        if (content == null)
        {
            throw new NotFoundException($"The content with language {request.Lang} does not exist");
        }

        context.Contents.Remove(content);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}