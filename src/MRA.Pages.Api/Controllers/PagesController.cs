using MediatR;
using Microsoft.AspNetCore.Mvc;
using MRA.Pages.Application.Contract.Content.Queries;
using MRA.Pages.Application.Contract.Page.Queries;

namespace MRA.Pages.Api.Controllers;

[Route("/api/[controller]")]
public class PagesController(ISender mediator)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pages = await mediator.Send(new GetPagesQuery());
        return Ok(pages);
    }

    [HttpGet(nameof(GetContent))]
    public async Task<IActionResult> GetContent([FromQuery] GetContentQuery query)
    {
        var contentResponse = await mediator.Send(query);
        return Ok(contentResponse);
    }
}