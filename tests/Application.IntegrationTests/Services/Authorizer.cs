using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MRA.Pages.Infrastructure.Identity;
using ClaimTypes = MRA.Configurations.Common.Constants.ClaimTypes;

namespace Application.IntegrationTests.Services;

public static class Authorizer
{
    public static string? JwtSecret { set; private get; }

    public static void ClearAuthorization(this HttpClient client)
    {
        client.DefaultRequestHeaders.Clear(); //todo clear token from cookie without clearing header;
    }

    public static async Task AddAuthorizationAsync(this HttpClient client, IEnumerable<Claim> claims)
    {
        var token = CreateJwt(claims);
        await client.GetAsync($"/pages/authorization/callback?atoken={token}");
    }

    public static void AddJwtAuthorization(this HttpClient client, IEnumerable<Claim> claims)
    {
        var token = CreateJwt(claims);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static string CreateJwt(IEnumerable<Claim> claims)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8
            .GetBytes(JwtSecret ?? throw new NullReferenceException("Jwt secret must be initialized")));

        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityToken token = new(
            claims: claims,
            expires: DateTime.Now.AddDays(2),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}

public class ClaimsBuilder
{
    private readonly List<Claim> _claims = [];

    public ClaimsBuilder AddClaim(string type, string value)
    {
        _claims.Add(new Claim(type, value));
        return this;
    }

    public static ClaimsBuilder New() => new();

    public ClaimsBuilder AddRole(string roleName) =>
        AddClaim(ClaimTypes.Role, roleName);

    public ClaimsBuilder AddSuperAdminRole() =>
        AddRole(ApplicationClaimValues.SuperAdministrator);

    public ClaimsBuilder AddApplication(string applicationName) =>
        AddClaim(ClaimTypes.Application, applicationName);

    public List<Claim> Build() => _claims;
}