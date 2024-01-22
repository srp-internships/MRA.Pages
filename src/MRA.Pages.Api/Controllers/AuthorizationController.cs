using Microsoft.AspNetCore.Mvc;

namespace MRA.Pages.Api.Controllers;

public class AuthorizationController : Controller
{
    public IActionResult CallBack(string atoken)
    {
        HttpContext.Response.Cookies.Append("authToken", atoken, new CookieOptions
        {
            Secure = false,
            Expires = DateTimeOffset.Now.AddDays(10),
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            IsEssential = false
        });
        return RedirectToAction("Index", "PagesView");
    }
}