using System.Net;
using System.Net.Http.Json;
using Application.IntegrationTests.Services;
using MRA.Pages.Application.Contract;
using MRA.Pages.Application.Contract.Page.Responses;

namespace Application.IntegrationTests.Page.Commands;

public class GetPagesQueryTests : BaseTest
{
    [Test]
    public async Task GetPages_WithoutAuth_ReturnsAll()
    {
        await AddAsync(GetPage("page1", "title", "en-US"));
        await AddAsync(GetPage("page2", "title", "en-US"));
        await AddAsync(GetPage("page3", "title", "en-US"));
        await AddAsync(GetPage("page4", "title", "en-US"));
        var response = await _httpClient.GetFromJsonAsync<List<PageResponse>>(Routes.Pages + "?lang=en-US");
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response?.FirstOrDefault(s => s.Name == "page1"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "page2"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "page3"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "page4"), Is.Not.Null);
        });
    }

    [Test]
    public async Task GetPages_GetWithRole_ReturnsPagesWithSuggestRoleAndLowerRoles()
    {
        await AddAsync(GetPage("page5", "title", "en-US", "Applicant,Reviewer,ApplicationAdmin"));
        await AddAsync(GetPage("page6", "title", "en-US", "Applicant,Reviewer,ApplicationAdmin"));
        await AddAsync(GetPage("page7", "title", "en-US", "ApplicationAdmin"));

        _httpClient.AddJwtAuthorization(ClaimsBuilder.New().AddRole("Reviewer").Build());
        var response = await _httpClient.GetFromJsonAsync<List<PageResponse>>(Routes.Pages + "?lang=en-US");
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response?.FirstOrDefault(s => s.Name == "page5"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "page6"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "page7"), Is.Null);
        });
    }

    [Test]
    public async Task GetPages_WithoutLangAndRole_ReturnsBadRequest()
    {
        await AddAsync(GetPage("page52", "title", "en-US", "Applicant,Reviewer,ApplicationAdmin"));
        await AddAsync(GetPage("page63", "title", "en-US", "Applicant,Reviewer,ApplicationAdmin"));
        await AddAsync(GetPage("page74", "title", "en-US", "ApplicationAdmin"));

        _httpClient.ClearAuthorization(); //without auth
        var response = await _httpClient.GetAsync(Routes.Pages); //without lang
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task GetPages_SetApplication_ReturnsSuggestApplicationPagesAndNullApplicationPages()
    {
        await AddAsync(GetPage("other", "title", "en-US"));
        await AddAsync(GetPage("other1", "title", "en-US"));
        await AddAsync(GetPage("application", "title", "en-US", default, "current"));
        await AddAsync(GetPage("application1", "title", "en-US", default, "other"));

        _httpClient.ClearAuthorization();
        var response =
            await _httpClient.GetFromJsonAsync<List<PageResponse>>(Routes.Pages + "?lang=en-US&Application=current");
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response?.FirstOrDefault(s => s.Name == "other"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "other1"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "application"), Is.Not.Null);
            Assert.That(response?.FirstOrDefault(s => s.Name == "application1"), Is.Null);
        });
    }


    private static MRA.Pages.Domain.Entities.Page GetPage(string name, string contentTitle, string contentLang,
        string? roles = null, string? application = null)
    {
        var page = new MRA.Pages.Domain.Entities.Page
        {
            Name = name,
            Application = application,
            Role = roles,
            ShowInMenu = false,
            Contents =
            [
                new MRA.Pages.Domain.Entities.Content
                {
                    Title = contentTitle,
                    Lang = contentLang
                }
            ]
        };
        return page;
    }
}