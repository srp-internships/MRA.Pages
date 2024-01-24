using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Infrastructure.Services;

namespace MRA.Pages.Api.Controllers;

public class AuthorizationController(IConfiguration configuration, JwtChecker checker) : Controller
{
    public async Task<IActionResult> CallBack(string atoken)
    {
        var success = await checker.LoginAsync(atoken);
        if (success)
        {
            return RedirectToAction("Index", "PagesView");
        }

        ViewBag.ErrorMessage = "Authorization failed";
        return View("ExtraPages/ErrorPage");
    }

    public IActionResult Login()
    {
        return Redirect(
            $"{configuration["MraIdentity-client"]}/login?callback={configuration["MraPages-hostname"]}/Authorization/callback");
    }
}