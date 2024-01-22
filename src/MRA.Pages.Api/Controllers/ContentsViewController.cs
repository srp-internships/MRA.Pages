using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Application.Contract.Content.Queries;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Api.Controllers;

[Authorize(Policy = ApplicationPolicies.SuperAdministrator)]
public class ContentViewController(ISender mediator)
    : Controller
{
    public async Task<IActionResult> Index(string pageName)
    {
        var contentResponses = await mediator.Send(new GetContentsQuery
        {
            PageName = pageName
        });
        ViewBag.ContentResponses = contentResponses;
        ViewBag.PageName = pageName;

        return View();
    }

    public IActionResult Create(string pageName)
    {
        ViewBag.PageName = pageName;
        return View();
    }
}