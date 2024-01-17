using Microsoft.EntityFrameworkCore;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Domain.Entities;

namespace MRA.Pages.Infrastructure.Persistence;
#nullable disable
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> opt) : DbContext(opt), IApplicationDbContext
{
    public DbSet<Page> Pages { get; set; }
    public DbSet<Content> Contents { get; set; }
}