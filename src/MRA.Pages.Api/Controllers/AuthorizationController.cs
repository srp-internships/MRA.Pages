using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MRA.Pages.Api.Controllers;

public class AuthorizationController(IConfiguration configuration) : Controller
{
    public async Task<IActionResult> CallBack(string atoken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var res = tokenHandler.ValidateToken(atoken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out _);
            var identity = new ClaimsIdentity(res.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignOutAsync();
            await HttpContext.SignInAsync(new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.MaxValue,
                    AllowRefresh = true
                });
        }
        catch (Exception)
        {
            return RedirectToAction("Login");
        }
        
        
        return RedirectToAction("Index", "PagesView");
    }

    public IActionResult Login()
    {
        return Redirect(
            $"{configuration["MraIdentity-client"]}/login?callback={configuration["MraPages-hostname"]}/Authorization/callback");
    }
}