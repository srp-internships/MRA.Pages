using System.Linq.Expressions;
using Application.IntegrationTests.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.Pages.Infrastructure.Persistence;

namespace Application.IntegrationTests;

[TestFixture]
public abstract class BaseTest
{
    // ReSharper disable once InconsistentNaming
    protected IConfiguration _configuration { get; private set; } = null!;

    // ReSharper disable once InconsistentNaming
    protected HttpClient _httpClient { get; private set; } = null!;

    // ReSharper disable once InconsistentNaming
    protected ISender _mediator { get; private set; } = null!;

    private CustomWebApplicationFactory _factory = null!;
    private ApplicationDbContext _context = null!;


    private void InitContext() =>
        _context = _factory.Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();


    [OneTimeSetUp]
    public virtual Task OneTimeSetup()
    {
        _factory = new CustomWebApplicationFactory();

        _configuration = _factory.Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();

        _mediator = _factory.Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope().ServiceProvider.GetRequiredService<ISender>();

        _httpClient = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            { HandleCookies = true, AllowAutoRedirect = false });

        Authorizer.JwtSecret = _configuration["JWT:Secret"];
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

    protected async Task<T?> FirsOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class
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