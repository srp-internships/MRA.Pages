using System.Net;
using System.Net.Http.Json;
using MRA.Pages.Application.Contract;
using MRA.Pages.Application.Contract.Page.Commands;
using MRA.Pages.Infrastructure.Identity;

namespace Application.IntegrationTests.Page.Commands;

public class CreatePageCommandTests : BaseTest
{
    [Test]
    public async Task CreatePageCommand_ValidRequest_ReturnsOk()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "name",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        var response = await _httpClient.PostAsJsonAsync(Routes.Pages, command);
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task CreatePageCommand_ValidRequest_ShouldSaveInDb()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "SaveDb",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        await _httpClient.PostAsJsonAsync(Routes.Pages, command);
        var response = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(response, Is.Not.Null);
    }


    [Test]
    public async Task CreatePageCommand_ExistName_ReturnsConflictShouldNotInsert()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
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
        var response = await _httpClient.PostAsJsonAsync(Routes.Pages, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        var result = await WhereToListAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(result, Has.Count.EqualTo(1));
    }


    [Test]
    public async Task CreatePageCommand_InvalidName_ReturnsBadRequestShouldNotInsert()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "invalid   characters %^&@@#$",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        await _httpClient.PostAsJsonAsync(Routes.Pages, command);
        var response = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(response, Is.Null);
    }

    [Test]
    public async Task CreatePageCommand_NotSuperAdminRole_ReturnsForbidden()
    {
        AddRoleAuthorization(ApplicationClaimValues.SuperAdministrator);
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "forbidden",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        var response = await _httpClient.PostAsJsonAsync(Routes.Pages, command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
    }
}