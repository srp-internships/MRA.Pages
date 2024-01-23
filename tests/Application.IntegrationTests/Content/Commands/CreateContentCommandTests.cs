using MRA.Pages.Application.Common.Exceptions;
using MRA.Pages.Application.Contract.Content.Commands;

namespace Application.IntegrationTests.Content.Commands;

public class CreateContentCommandTests : BaseTest
{
    private MRA.Pages.Domain.Entities.Page _page = null!;

    [Test]
    public async Task CreateContentCommand_ValidRequest_ReturnsOkShouldSaveInDb()
    {
        await SetPage("1");
        var command = new CreateContentCommand
        {
            PageName = _page.Name,
            Title = "title",
            Lang = "ru",
            HtmlContent = "fasdfasdf"
        };
        Assert.DoesNotThrowAsync(async () => await _mediator.Send(command));
        var response = await FirsAllDefaultAsync<MRA.Pages.Domain.Entities.Content>(s => s.Lang == command.Lang);
        Assert.That(response, Is.Not.Null);
    }

    [Test]
    public async Task CreateContentCommand_ExistLang_ShouldNotInsertThrowsConflict()
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
        Assert.ThrowsAsync<ConflictException>(async () => await _mediator.Send(command));
        
        var result =
            await WhereToListAsync<MRA.Pages.Domain.Entities.Content>(s =>
                s.PageId == _page.Id || s.Lang == command.Lang);
        Assert.That(result, Has.Count.EqualTo(1));
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