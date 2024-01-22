using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Application.Contract.Content.Commands;

namespace MRA.Pages.Application.Features.Content.Commands;

public class CreateContentCommandHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<CreateContentCommand, Unit>
{
    public async Task<Unit> Handle(CreateContentCommand request, CancellationToken cancellationToken)
    {
        var pageId = await context.Pages.Where(s => s.Name == request.PageName).Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (pageId.Equals(Guid.Empty))
        {
            throw new NotFoundException($"page with name {request.PageName} not found");
        }

        if (await context.Contents.AnyAsync(s => s.PageId == pageId && s.Lang == request.Lang, cancellationToken))
        {
            throw new ConflictException(
                $"the content with language {request.Lang} in page {request.PageName} already exist");
        }

        var content = mapper.Map<Domain.Entities.Content>(request);
        content.PageId = pageId;
        await context.Contents.AddAsync(content, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}