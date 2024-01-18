namespace MRA.Pages.Application.Contract.Pages.Commands;

public class CreatePageCommand : IRequest<string>
{
    public bool Disabled { get; set; }
    public required string Name { get; set; }
    public string Application { get; set; } = "";
    public string Role { get; set; } = "";
    public bool ShowInMenu { get; set; }
}