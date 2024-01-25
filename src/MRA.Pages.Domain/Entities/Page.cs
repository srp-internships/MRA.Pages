namespace MRA.Pages.Domain.Entities;

public class Page
{
    public Guid Id { get; set; }
    public bool Disabled { get; set; }
    public required string Name { get; set; }
    public string? Application { get; set; }
    public string? Role { get; set; }
    public bool ShowInMenu { get; set; }
    public ICollection<Content>? Contents { get; set; }
}