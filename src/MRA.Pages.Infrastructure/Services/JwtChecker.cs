using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MRA.Pages.Infrastructure.Services;

public class JwtChecker(IHttpContextAccessor httpContextAccessor,IConfiguration configuration,Logger<JwtChecker> logger)
{
    public async Task<bool> LoginAsync(string jwtToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var res = tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out _);
            var identity = new ClaimsIdentity(res.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await httpContextAccessor.HttpContext!.SignOutAsync();
            await httpContextAccessor.HttpContext!.SignInAsync(new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.MaxValue,
                    AllowRefresh = true
                });
            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e,"error when checking token");
            return false;
        }
    }
}