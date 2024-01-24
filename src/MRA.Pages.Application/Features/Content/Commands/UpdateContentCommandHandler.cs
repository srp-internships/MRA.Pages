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
        var old = await context.Pages.FirstOrDefaultAsync(s => s.Name == request.OldLang, cancellationToken);
        if (old == null)
        {
            throw new NotFoundException($"can't find page with name {request.OldLang}");
        }

        mapper.Map(request, old);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}