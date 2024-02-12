using Application.IntegrationTests.Services;
using MRA.Pages.Application.Contract.Content.Commands;

namespace Application.IntegrationTests.Content.Commands;

public class DeleteContentCommandTests : BaseTest
{
    private const string DeleteContentEndPoint = "/pages/contentView/Delete";
    private const string DefaultPageName = "nameUnique1234";
    private const string DefaultLang = "lang";
    private Guid _createdPageId;

    [SetUp]
    public async Task Setup()
    {
        await _httpClient.AddAuthorizationAsync(ClaimsBuilder.New().AddSuperAdminRole().Build());
        _createdPageId = await CreatePage(DefaultPageName);
        await CreateContent(DefaultLang, _createdPageId);
    }

    [Test]
    public async Task CreatePageCommand_ValidRequest_RedirectToIndex()
    {
        var command = new DeleteContentCommand
        {
            Lang = DefaultLang,
            PageName = DefaultPageName
        };
        var response = await _httpClient.PostAsFormAsync(DeleteContentEndPoint, command);
        Assert.That(response.Headers.Location?.ToString(), Does.Contain("/pages/contentView").IgnoreCase);
    }

    [Test]
    public async Task CreateContentCommand_ValidRequest_ShouldRemoveFromDb()
    {
        var command = new DeleteContentCommand
        {
            Lang = DefaultLang,
            PageName = DefaultPageName
        };
        await _httpClient.PostAsFormAsync(DeleteContentEndPoint, command);
        var response =
            await FirsOrDefaultAsync<MRA.Pages.Domain.Entities.Content>(s =>
                s.Lang == command.Lang && s.PageId == _createdPageId);
        Assert.That(response, Is.Null);
    }

    private async Task<Guid> CreatePage(string name)
    {
        var page = await FirsOrDefaultAsync<MRA.Pages.Domain.Entities.Page>(p => p.Name == name);
        if (page == null)
        {
            page = new MRA.Pages.Domain.Entities.Page
            {
                Name = name
            };
            await AddAsync(page);
        }

        return page.Id;
    }

    private async Task CreateContent(string lang, Guid pageId)
    {
        var content =
            await FirsOrDefaultAsync<MRA.Pages.Domain.Entities.Content>(p => p.PageId == pageId && p.Lang == lang);
        if (content == null)
        {
            content = new MRA.Pages.Domain.Entities.Content
            {
                Title = "this is a title",
                Lang = lang,
                PageId = pageId
            };
            await AddAsync(content);
        }
    }
}