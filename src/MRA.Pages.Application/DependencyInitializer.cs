using Microsoft.Extensions.DependencyInjection;

namespace MRA.Pages.Application;

public static class DependencyInitializer
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}