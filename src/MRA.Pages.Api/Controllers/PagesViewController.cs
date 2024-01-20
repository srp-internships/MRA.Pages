using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Application.Contract.Page.Queries;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Api.Controllers;

public class PagesViewController(ISender mediator)
    : Controller
{
    [Authorize(Policy = ApplicationPolicies.SuperAdministrator)]
    public async Task<IActionResult> Index(GetPagesQuery? query = null)
    {
        var result = await mediator.Send(query ?? new GetPagesQuery());
        ViewBag.PageResponses = result;
        return View();
    }
}