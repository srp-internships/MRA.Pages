using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Page.Commands;

namespace MRA.Pages.Application.Features.Page.Commands;

public class DeletePageCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeletePageCommand, Unit>
{
    public async Task<Unit> Handle(DeletePageCommand request, CancellationToken cancellationToken)
    {
        var page = await context.Pages.FirstOrDefaultAsync(p => p.Name == request.Name, cancellationToken);
        if (page == null)
        {
            throw new NotFoundException($"The page with name {request.Name} does not exist");
        }

        context.Pages.Remove(page);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}