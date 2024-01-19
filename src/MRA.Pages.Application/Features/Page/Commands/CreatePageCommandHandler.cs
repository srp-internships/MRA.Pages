using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Page.Commands;

namespace MRA.Pages.Application.Features.Page.Commands;

public class CreatePageCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreatePageCommand, Unit>
{
    public async Task<Unit> Handle(CreatePageCommand request, CancellationToken cancellationToken)
    {
        if (await context.Pages.AnyAsync(s => s.Name.ToLower() == request.Name.ToLower(),
                cancellationToken: cancellationToken))
        {
            throw new ConflictException($"Page with name {request.Name} already exists");
        }

        await context.Pages.AddAsync(mapper.Map<Domain.Entities.Page>(request), cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}