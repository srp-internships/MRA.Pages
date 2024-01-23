using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Application.Contract.Page.Commands;
using MRA.Pages.Application.Contract.Page.Queries;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Api.Controllers;

[Authorize(Policy = ApplicationPolicies.SuperAdministrator)]
public class PagesViewController(ISender mediator)
    : Controller
{
    public async Task<IActionResult> Index(GetPagesQuery? query = null)
    {
        var result = await mediator.Send(query ?? new GetPagesQuery());
        ViewBag.PageResponses = result;
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePageCommand command)
    {
        await mediator.Send(command);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string pageName)
    {
        var pageResponse = await mediator.Send(new GetUpdatePageCommandQuery
        {
            Name = pageName
        });
        var model = new UpdatePageCommand
        {
            OldName = pageName,
            Disabled = pageResponse.Disabled,
            Name = pageResponse.Name,
            Application = pageResponse.Application,
            Role = pageResponse.Role,
            ShowInMenu = pageResponse.ShowInMenu
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdatePageCommand updatePageCommand)
    {
        await mediator.Send(updatePageCommand);
        return RedirectToAction("Index");
    }
}