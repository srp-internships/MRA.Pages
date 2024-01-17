namespace MRA.Pages.Domain.Entities;

public class Page : BaseEntity
{
    public required string Name { get; set; }
    public string Role { get; set; } = "";
    public bool ShowInMenu { get; set; }
    public ICollection<Content>? Contents { get; set; }
}