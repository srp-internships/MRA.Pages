using Microsoft.AspNetCore.Mvc;

namespace MRA.Pages.Api.Controllers;

public class ExtraController : Controller
{
    public IActionResult ErrorPage()
    {
        return View("ExtraPages/ErrorPage");
    }

    public IActionResult Forbidden()
    {
        return View("ExtraPages/Forbidden");
    }
}