using System.Net;
using System.Net.Http.Json;
using Application.IntegrationTests.Services;
using MRA.Pages.Application.Contract;
using MRA.Pages.Application.Contract.Content.Responses;

namespace Application.IntegrationTests.Content.Commands;

public class GetContentQueryTests : BaseTest
{
    private MRA.Pages.Domain.Entities.Page _page = null!;

    [Test]
    public async Task GetContent_ValidRequest_ReturnSContentResult()
    {
        await InitPageAsync("newPage");
        await AddContent("title", "ru-RU", "this is html content");

        _httpClient.ClearAuthorization();
        var content =
            await _httpClient.GetFromJsonAsync<ContentResponse>(Routes.Contents + "?pageName=newPage&lang=ru-RU");
        Assert.Multiple(() =>
        {
            Assert.That(content?.HtmlContent, Is.EqualTo("this is html content"));
            Assert.That(content?.Title, Is.EqualTo("title"));
        });
    }

    [Test]
    public async Task GetContent_WithoutRequiredRole_ReturnsForbidden()
    {
        await InitPageAsync("newPage1", "role");
        await AddContent("title1", "ru-RU", "this is html content");

        _httpClient.ClearAuthorization();
        var content =
            await _httpClient.GetAsync(Routes.Contents + "?pageName=newPage1&lang=ru-RU");
        Assert.That(content.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    [Test]
    public async Task GetContent_WithRequiredRole_ReturnsContent()
    {
        await InitPageAsync("newPage12", "Applicant,Reviewer");
        await AddContent("title13", "ru-RU", "this is html content");

        _httpClient.AddJwtAuthorization(ClaimsBuilder.New().AddRole("Applicant").Build());
        var content =
            await _httpClient.GetFromJsonAsync<ContentResponse>(Routes.Contents + "?lang=ru-RU&pageName=newPage12");
        Assert.Multiple(() =>
        {
            Assert.That(content?.HtmlContent, Is.EqualTo("this is html content"));
            Assert.That(content?.Title, Is.EqualTo("title13"));
        });
    }

    [Test]
    public async Task GetContent_GetNotExistContent_ReturnsNotFound()
    {
        await InitPageAsync("newPage123");
        await AddContent("title13", "ru-RU1", "this is html content");

        _httpClient.ClearAuthorization();
        var content =
            await _httpClient.GetAsync(Routes.Contents + "?lang=ru-RU&pageName=newPage12");
        Assert.That(content.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }


    private async Task InitPageAsync(string name, string? role = null, string? application = null)
    {
        var page = new MRA.Pages.Domain.Entities.Page
        {
            Name = name,
            Application = application,
            Role = role,
            ShowInMenu = false,
        };
        await AddAsync(page);
        _page = page;
    }

    private async Task AddContent(string title, string lang, string htmlContent = "")
    {
        var content = new MRA.Pages.Domain.Entities.Content
        {
            HtmlContent = htmlContent,
            Title = title,
            Lang = lang,
            PageId = _page.Id
        };
        await AddAsync(content);
    }
}