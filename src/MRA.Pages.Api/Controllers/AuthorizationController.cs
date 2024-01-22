using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace MRA.Pages.Api.Controllers;

public class AuthorizationController : Controller
{
    private readonly IConfiguration _configuration;
    public AuthorizationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<IActionResult> CallBack(string atoken)
    {
        //var tokenHandler = new JwtSecurityTokenHandler();
        //try
        //{
        //    var res = tokenHandler.ValidateToken(atoken, new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = false,
        //        IssuerSigningKey =
        //            new SymmetricSecurityKey(
        //                Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)),
        //        ValidateIssuer = false,
        //        ValidateAudience = false
        //    }, out _);
        //}
        //catch (Exception ex)
        //{

        //}
        //var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id.ToString()), new Claim("Email", user.Email) }, CookieAuthenticationDefaults.AuthenticationScheme);

        //await HttpContext.SignOutAsync();
        //await HttpContext.SignInAsync(new ClaimsPrincipal(identity),
        //    new AuthenticationProperties
        //    {
        //        IsPersistent = true,
        //        ExpiresUtc = DateTimeOffset.MaxValue,
        //        AllowRefresh = true
        //    });
        return RedirectToAction("Index", "PagesView");
    }

    public IActionResult Login()
    {
        return Redirect("https://localhost:7081/login?callback=https://localhost:7154/authorization/callback");
    }
}