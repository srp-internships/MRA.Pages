using MRA.Configurations.Initializer.Azure.AppConfig;
using MRA.Configurations.Initializer.Azure.KeyVault;
using MRA.Pages.Api;
using MRA.Pages.Application;
using MRA.Pages.Infrastructure;
using MRA.Pages.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.ConfigureAzureKeyVault("MraPages");
    string appConfigConnectionString = builder.Configuration["AppConfigConnectionString"]!;
    builder.Configuration.AddAzureAppConfig(appConfigConnectionString);
}

builder.Services.AddApiServices(builder.Environment.IsDevelopment());
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

app.MapControllerRoute(
    name: "default",
    pattern: "pages/{controller=PagesView}/{action=Index}/{id?}");

app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program;