using Microsoft.AspNetCore.Mvc;

namespace MRA.Pages.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class PagesController
{
    [HttpGet]
    public Task<IActionResult> Get([FromQuery] string lang="ru-Ru")
    {
        return Task.FromResult<IActionResult>(new ContentResult
        {
            Content = "test",
            ContentType = "text/pain",
            StatusCode = 200
        });
    }
}