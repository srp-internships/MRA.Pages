using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Page.Commands;

namespace MRA.Pages.Application.Features.Page.Commands;

public class UpdatePageCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<UpdatePageCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
    {
        var old = await context.Pages.FirstOrDefaultAsync(s => s.Name == request.OldName, cancellationToken);
        if (old == null)
        {
            throw new NotFoundException($"can't find page with name {request.OldName}");
        }

        mapper.Map(request, old);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}