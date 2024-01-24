using MRA.Pages.Api.Filters;

namespace MRA.Pages.Api;

public static class DependencyInitializer
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers(opt => { opt.Filters.Add<ApiExceptionFilter>(); });
        return services;
    }
}