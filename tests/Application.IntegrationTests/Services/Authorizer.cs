using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
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
        var response = await client.GetAsync($"/Authorization/callback?token={token}");

        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            var redirectUri = response.Headers.Location;
            response = await client.GetAsync(redirectUri);
            var cookies = response.Headers.GetValues("Set-Cookie");

            const string targetCookieName = ".AspNetCore.Cookies";

            var targetCookieValue = cookies
                .Where(cookie => cookie.Equals(targetCookieName))
                .Select(cookie => cookie.Split(';')[0])
                .FirstOrDefault() ?? throw new AuthenticationException();

            client.DefaultRequestHeaders.Add("Cookie", targetCookieValue);
        }

        throw new AuthenticationException();
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

    public ClaimsBuilder AddApplication(string applicationName) =>
        AddClaim(ClaimTypes.Application, applicationName);

    public List<Claim> Build() => _claims;
}