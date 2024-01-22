using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MRA.Pages.Application;

public static class DependencyInitializer
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        return services;
    }
}