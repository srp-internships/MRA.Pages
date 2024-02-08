using MRA.Pages.Api.Controllers;
using MRA.Pages.Api.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MRA.Pages.Api;

public static class DependencyInitializer
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, bool isDevelopmentMode)
    {
        services.AddControllersWithViews();
        services.AddEndpointsApiExplorer();
        if (isDevelopmentMode)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (apiDesc.TryGetMethodInfo(out var methodInfo))
                    {
                        return methodInfo.DeclaringType?.Name == nameof(PagesController);
                    }

                    return false;
                });
            });
        }

        services.AddControllers(opt => { opt.Filters.Add<ApiExceptionFilter>(); });
        return services;
    }
}