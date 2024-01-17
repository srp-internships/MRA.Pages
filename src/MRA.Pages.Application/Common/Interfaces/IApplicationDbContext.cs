using Microsoft.EntityFrameworkCore;
using MRA.Pages.Domain.Entities;

namespace MRA.Pages.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<Page> Pages { get; set; }
    public DbSet<Content> Contents { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}