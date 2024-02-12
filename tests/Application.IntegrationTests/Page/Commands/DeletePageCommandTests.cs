using Application.IntegrationTests.Services;
using MRA.Pages.Application.Contract.Page.Commands;

namespace Application.IntegrationTests.Page.Commands;

public class DeletePageCommandTests : BaseTest
{
    private const string DeletePageEndPoint = "/pages/PagesView/Delete";
    private const string DefaultPageName = "name";

    [SetUp]
    public async Task Setup()
    {
        await _httpClient.AddAuthorizationAsync(ClaimsBuilder.New().AddSuperAdminRole().Build());
        await CreatePage("name");
    }

    [Test]
    public async Task CreatePageCommand_ValidRequest_RedirectToIndex()
    {
        var command = new DeletePageCommand
        {
            Name = DefaultPageName
        };
        var response = await _httpClient.PostAsFormAsync(DeletePageEndPoint, command);
        Assert.That(response.Headers.Location?.ToString(), Is.EqualTo("/pages/pagesView"));
    }

    [Test]
    public async Task CreatePageCommand_ValidRequest_ShouldRemoveFromDb()
    {
        var command = new DeletePageCommand
        {
            Name = DefaultPageName
        };
        await _httpClient.PostAsFormAsync(DeletePageEndPoint, command);
        var response = await FirsOrDefaultAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(response, Is.Null);
    }

    private async Task CreatePage(string name)
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
    }
}