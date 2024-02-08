using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Infrastructure.Services;

namespace MRA.Pages.Api.Controllers;

public class AuthorizationController(
    IConfiguration configuration,
    JwtChecker checker,
    ILogger<AuthorizationController> logger) : Controller
{
    public async Task<IActionResult> CallBack(string atoken)
    {
        logger.LogInformation("callback requested");
        var success = await checker.LoginAsync(atoken);
        if (success)
        {
            ViewBag.RedirectUrl = Url.Action("Index", "PagesView")!;
            return View("ExtraPages/Redirect");
        }

        ViewBag.ErrorMessage = "Authorization failed";
        return View("ExtraPages/ErrorPage");
    }

    public IActionResult Login()
    {
        ViewBag.AuthorizationLink =
            $"{configuration["MraIdentityClient-HostName"]}/login?callback={configuration["MraPages-HostName"]}/pages/Authorization/callback";
        return View();
    }
}