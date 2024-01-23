using System.Net;
using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Contract;
using MRA.Pages.Application.Contract.Page.Commands;

namespace Application.IntegrationTests.Page.Commands;

public class CreatePageCommandTests : BaseTest
{
    [Test]
    public async Task CreatePageCommand_ValidRequest_ReturnsOk()
    {
        var command = new CreatePageCommand
        {
            Disabled = false,
            Name = "name",
            Application = "",
            Role = "",
            ShowInMenu = true
        };
        Assert.DoesNotThrowAsync(async () => await _mediator.Send(command));

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
        Assert.ThrowsAsync<ConflictException>(async () => await _mediator.Send(command));
        
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
        var result = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Page>(s => s.Name == command.Name);
        Assert.That(result, Is.Null);
    }
}