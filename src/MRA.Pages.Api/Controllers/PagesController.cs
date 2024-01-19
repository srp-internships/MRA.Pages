using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Application.Contract.Page.Commands;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Api.Controllers;

[Route("/api/[controller]")]
public class PagesController(ISender mediator)
    : Controller
{
    [Authorize]
    [HttpGet]
    public Task<IActionResult> Get([FromQuery] string lang = "ru-Ru")
    {
        return Task.FromResult<IActionResult>(new ContentResult
        {
            Content = "test",
            ContentType = "text/pain",
            StatusCode = 200
        });
    }

    [HttpPost]
    [Authorize(Policy = ApplicationPolicies.SuperAdministrator)]
    public async Task<IActionResult> CreatePage([FromBody] CreatePageCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}