using MRA.Pages.Application;
using MRA.Pages.Infrastructure;
using MRA.Pages.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


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
app.MapControllers();
app.UseCors("CORS_POLICY");

app.UseAuthentication();
app.UseAuthorization();

app.Run();

public partial class Program;