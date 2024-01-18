namespace MRA.Pages.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    public Task SeedAsync()
    {
        return Task.CompletedTask;
    }
}