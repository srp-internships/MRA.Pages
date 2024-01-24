using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Application.Contract.Content.Commands;
using MRA.Pages.Application.Contract.Content.Queries;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Api.Controllers;

[Authorize(Policy = ApplicationPolicies.SuperAdministrator,
    AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class ContentViewController(ISender mediator, IMapper mapper)
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

    [HttpPost]
    public async Task<IActionResult> Create(CreateContentCommand command)
    {
        await mediator.Send(command);
        return Redirect($"{Url.Action("Index")}?pageName={command.PageName}");
    }

    public async Task<IActionResult> Edit(string lang, string pageName)
    {
        var model = await mediator.Send(new GetContentQuery
        {
            PageName = pageName,
            Lang = lang
        });
        return View(mapper.Map<UpdateContentCommand>(model));
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateContentCommand command)
    {
        await mediator.Send(command);
        return Redirect($"{Url.Action("Index")}?pageName={command.PageName}");
    }
}