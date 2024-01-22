using MRA.Pages.Api;
using MRA.Pages.Application;
using MRA.Pages.Infrastructure;
using MRA.Pages.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var migrator = scope.ServiceProvider.GetRequiredService<DbMigration>();
    await migrator.InitialiseAsync();

    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
    await initializer.SeedAsync();
}


app.UseHttpsRedirection();

app.UseCors("CORS_POLICY");


app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy();

app.Use(async (context, next) =>
{
    await next(context);
    if (!context.Request.Path.StartsWithSegments("/api") && context.Response.StatusCode is 401 or 403)
    {
        context.Response.Redirect(
            $"{builder.Configuration["MraIdentity-client"]}/login?callback={builder.Configuration["MraPages-hostname"]}/Authorization/callback");
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PagesView}/{action=Index}/{id?}");

app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program;