using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Claims;
using Application.IntegrationTests.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRA.Pages.Infrastructure.Persistence;
using ClaimTypes = MRA.Configurations.Common.Constants.ClaimTypes;

namespace Application.IntegrationTests;

[TestFixture]
public class BaseTest
{
    // ReSharper disable once InconsistentNaming
    protected HttpClient _httpClient => _factory.CreateDefaultClient();
    private CustomWebApplicationFactory _factory = null!;
    private readonly JwtTokenService _tokenService = new();
    private ApplicationDbContext _context = null!;
    private string _jwtSecret = "";

    private void InitContext() =>
        _context = _factory.Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();


    [OneTimeSetUp]
    public virtual Task OneTimeSetup()
    {
        _factory = new CustomWebApplicationFactory();

        var configuration = _factory.Services.GetRequiredService<IServiceScopeFactory>()
            .CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
        _jwtSecret = configuration["JWT:Secret"]!;
        return Task.CompletedTask;
    }

    protected void AddAuthorization()
    {
        using var scope = _factory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var token = _tokenService.CreateTokenByClaims(_jwtSecret);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _tokenService.Dispose();
    }


    protected void AddRoleAuthorization(string role)
    {
        using var scope = _factory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var token = _tokenService.CreateTokenByClaims(_jwtSecret, [new Claim(ClaimTypes.Role, role)]);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _tokenService.Dispose();
    }

    protected void AddApplicationAuthorization(string application)
    {
        using var scope = _factory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var token = _tokenService.CreateTokenByClaims(_jwtSecret, [new Claim(ClaimTypes.Role, application)]);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _tokenService.Dispose();
    }

    protected void AddAuthorization(string application, string role)
    {
        using var scope = _factory.Services.GetService<IServiceScopeFactory>()!.CreateScope();
        var token = _tokenService.CreateTokenByClaims(_jwtSecret,
        [
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Application, application)
        ]);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _tokenService.Dispose();
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