using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MRA.Pages.Infrastructure.Persistence;

namespace Application.IntegrationTests;

[TestFixture]
public class BaseTest
{
    protected HttpClient Http => _factory.CreateDefaultClient();
    private CustomWebApplicationFactory _factory = null!;
    private ApplicationDbContext _context = null!;

    private void InitContext() =>
        _context = _factory.Services.GetRequiredService<ApplicationDbContext>();


    [OneTimeSetUp]
    public virtual Task OneTimeSetup()
    {
        _factory = new CustomWebApplicationFactory();
        return Task.CompletedTask;
    }

    protected async Task AddAsync<T>(T entity) where T : class
    {
        InitContext();
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    protected async Task RemoveAsync<T>(T entity) where T : class
    {
        InitContext();
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    protected async Task<T?> FirsAllDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        InitContext();
        var firstOrDefaultAsync = await _context.Set<T>().FirstOrDefaultAsync(predicate);
        return firstOrDefaultAsync;
    }

    protected async Task<List<T>> WhereToListAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        InitContext();
        var firstOrDefaultAsync = await _context.Set<T>().Where(predicate).ToListAsync();
        return firstOrDefaultAsync;
    }


    protected async Task ClearAsync<T>() where T : class
    {
        InitContext();
        _context.Set<T>().RemoveRange(await _context.Set<T>().ToArrayAsync());
        await _context.SaveChangesAsync();
    }
}