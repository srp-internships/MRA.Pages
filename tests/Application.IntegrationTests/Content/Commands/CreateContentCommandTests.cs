using System.Net;
using Application.IntegrationTests.Services;
using MRA.Pages.Application.Contract.Content.Commands;

namespace Application.IntegrationTests.Content.Commands;

public class CreateContentCommandTests : BaseTest
{
    private MRA.Pages.Domain.Entities.Page _page = null!;
    private const string CreateContentUrl = "/contentView/Create";

    [SetUp]
    public async Task Setup()
    {
        await _httpClient.AddAuthorizationAsync(ClaimsBuilder.New().AddSuperAdminRole().Build());
    }

    [Test]
    public async Task CreateContentCommand_ValidRequest_ReturnsOk()
    {
        await SetPage("1");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "title",
            Lang = "ru",
            HtmlContent = "fasdfasdf"
        };
        var response = await _httpClient.PostAsFormAsync(CreateContentUrl, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
    }

    [Test]
    public async Task CreateContentCommand_ValidRequest_ShouldSaveInDb()
    {
        await SetPage("afsd");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "ad",
            Lang = "fad",
            HtmlContent = "fasdfasdfsasdf"
        };
        await _httpClient.PostAsFormAsync(CreateContentUrl, command);
        var response = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Content>(s => s.Lang == command.Lang);
        Assert.That(response, Is.Not.Null);
    }


    [Test]
    public async Task CreateContentCommand_ExistLang_ShouldNotInsert()
    {
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
        await _httpClient.PostAsFormAsync(CreateContentUrl, command);

        var result =
            await WhereToListAsync<MRA.Pages.Domain.Entities.Content>(s =>
                s.PageId == _page.Id || s.Lang == command.Lang);
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task CreateContentCommand_NotSuperAdminRole_ReturnsForbidden()
    {
        await SetPage("1rew3");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "title",
            Lang = "ru",
            HtmlContent = "fasdfasdf"
        };
        var response = await _httpClient.PostAsFormAsync(CreateContentUrl, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
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