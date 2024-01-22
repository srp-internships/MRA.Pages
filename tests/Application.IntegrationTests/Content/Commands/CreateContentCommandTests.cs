using System.Net;
using System.Net.Http.Json;
using MRA.Pages.Application.Contract;
using MRA.Pages.Application.Contract.Content.Commands;
using MRA.Pages.Infrastructure.Identity;

namespace Application.IntegrationTests.Content.Commands;

public class CreateContentCommandTests : BaseTest
{
    private MRA.Pages.Domain.Entities.Page _page = null!;

    [Test]
    public async Task CreateContentCommand_ValidRequest_ReturnsOk()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
        await SetPage("1");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "title",
            Lang = "ru",
            HtmlContent = "fasdfasdf"
        };
        var response = await _httpClient.PostAsJsonAsync(Routes.Contents, command);
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task CreateContentCommand_ValidRequest_ShouldSaveInDb()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
        await SetPage("12");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "titadsle",
            Lang = "ru-en",
            HtmlContent = "fasdfasdfsasdf"
        };
        await _httpClient.PostAsJsonAsync(Routes.Contents, command);
        var response = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Content>(s => s.Lang == command.Lang);
        Assert.That(response, Is.Not.Null);
    }


    [Test]
    public async Task CreateContentCommand_ExistLang_ReturnsConflictShouldNotInsert()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
        await SetPage("13");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "title",
            Lang = "ru",
            HtmlContent = "fasdfasdf"
        };
        await AddAsync(new MRA.Pages.Domain.Entities.Content
        {
            Lang = command.Lang,
            PageId = _page.Id,
            Title = command.Title
        });
        var response = await _httpClient.PostAsJsonAsync(Routes.Contents, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        var result =
            await WhereToListAsync<MRA.Pages.Domain.Entities.Content>(s =>
                s.PageId == _page.Id || s.Lang == command.Lang);
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CreateContentCommand_NotSuperAdminRole_ReturnsForbidden()
    {
        AddRoleAuthorization(ApplicationClaimValues.Administrator);
        await SetPage("1rew3");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "title",
            Lang = "ru",
            HtmlContent = "fasdfasdf"
        };
        var response = await _httpClient.PostAsJsonAsync(Routes.Contents, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }

    private async Task SetPage(string name)
    {
        var page = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == name);
        if (page == null)
        {
            page = new MRA.Pages.Domain.Entities.Page
            {
                Name = name,
            };
            await AddAsync(page);
        }

        _page = page;
    }
}