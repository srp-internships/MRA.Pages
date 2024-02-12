using System.Net;
using Application.IntegrationTests.Services;
using MRA.Pages.Application.Contract.Page.Commands;
using MRA.Pages.Infrastructure.Identity;

namespace Application.IntegrationTests.Page.Commands;

public class CreatePageCommandTests : BaseTest
{
    private const string CreatePageEndPoint = "/pages/PagesView/Create";

    [SetUp]
    public async Task Setup()
    {
        await _httpClient.AddAuthorizationAsync(ClaimsBuilder.New().AddSuperAdminRole().Build());
    }

    [Test]
    public async Task CreatePageCommand_ValidRequest_RedirectToIndex()
    {
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "name",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        var response = await _httpClient.PostAsFormAsync(CreatePageEndPoint, command);
        Assert.That(response.Headers.Location?.ToString(), Is.EqualTo("/pages/pagesView"));
    }

    [Test]
    public async Task CreatePageCommand_ValidRequest_ShouldSaveInDb()
    {
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "SaveDb",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        await _httpClient.PostAsFormAsync(CreatePageEndPoint, command);
        var response = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(response, Is.Not.Null);
    }


    [Test]
    public async Task CreatePageCommand_ExistName_ReturnsConflictShouldNotInsert()
    {
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "ConflictTest",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        await AddAsync(new MRA.Pages.Domain.Entities.Page
        {
            Name = command.Name
        });
        var response = await _httpClient.PostAsFormAsync(CreatePageEndPoint, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        var result = await WhereToListAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(result, Has.Count.EqualTo(1));
    }


    [Test]
    [Ignore("undefined")]
    public async Task CreatePageCommand_InvalidName_ReturnsBadRequestShouldNotInsert()
    {
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "invalid   characters %^&@@#$",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        var response = await _httpClient.PostAsFormAsync(CreatePageEndPoint, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var result = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(result, Is.Null);
    }

    [TestCase(ApplicationClaimValues.Administrator)]
    [TestCase(ApplicationClaimValues.Reviewer)]
    [TestCase("")]
    public async Task CreatePageCommand_NotSuperAdminRole_RedirectToForbiddenPage(string role)
    {
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "forbidden",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        _httpClient.ClearAuthorization();
        await _httpClient.AddAuthorizationAsync(ClaimsBuilder.New().AddRole(role).Build());
        var response = await _httpClient.PostAsFormAsync(CreatePageEndPoint, command);
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect));
            Assert.That(response.Headers.Location?.ToString(), Does.Contain("/extra/forbidden").IgnoreCase);
        });
    }
}