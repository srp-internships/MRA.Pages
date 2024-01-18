using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MRA.Pages.Application.Common.Interfaces;
using MRA.Pages.Infrastructure.Identity;
using MRA.Pages.Infrastructure.Persistence;

namespace MRA.Pages.Infrastructure;

public static class DependencyInitializer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var dbConnectionString = configuration.GetConnectionString("DefaultConnection")!;
            if (configuration["UseInMemoryDatabase"] == "true")
                options.UseInMemoryDatabase("testDB");
            else
                options.UseSqlServer(dbConnectionString);
        });
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ApplicationDbContextInitializer>();
        services.AddScoped<DbMigration>();
        return services.AddSecurityProviders(configuration);
    }


    private static IServiceCollection AddSecurityProviders(this IServiceCollection services,
        IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(op =>
        {
            op.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build())
            .AddPolicy(ApplicationPolicies.SuperAdministrator, op => op
                .RequireRole(ApplicationClaimValues.SuperAdministrator)
                .RequireClaim(ClaimTypes.Application))
            .AddPolicy(ApplicationPolicies.Administrator, op => op
                .RequireRole(ApplicationClaimValues.SuperAdministrator, ApplicationClaimValues.Administrator)
                .RequireClaim(ClaimTypes.Application))
            .AddPolicy(ApplicationPolicies.Reviewer, op => op
                .RequireRole(ApplicationClaimValues.Reviewer)
                .RequireClaim(ClaimTypes.Application));

        var corsAllowedHosts = configuration.GetSection("MraPages-CORS").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy("CORS_POLICY", policyConfig =>
            {
                policyConfig.WithOrigins(corsAllowedHosts!)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        return services;
    }
}