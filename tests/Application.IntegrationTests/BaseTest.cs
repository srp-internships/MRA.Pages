namespace Application.IntegrationTests;

[TestFixture]
public class BaseTest
{
    private CustomWebApplicationFactory _factory = null!;

    [OneTimeSetUp]
    public virtual Task OneTimeSetup()
    {
        _factory = new CustomWebApplicationFactory();
        return Task.CompletedTask;
    }
}