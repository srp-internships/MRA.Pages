using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Application.Contract.Content.Commands;
using MRA.Pages.Application.Contract.Content.Queries;
using MRA.Pages.Infrastructure.Identity;

namespace MRA.Pages.Api.Controllers;

[Route("/api/[controller]")]
public class ContentsController(ISender mediator)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetContent([FromQuery] GetContentQuery query)
    {
        var contentResponse = await mediator.Send(query);
        return Ok(contentResponse);
    }

    [Authorize(Policy = ApplicationPolicies.SuperAdministrator)]
    [HttpPost]
    public async Task<IActionResult> CreateContent([FromBody] CreateContentCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}