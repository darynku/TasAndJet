using Microsoft.EntityFrameworkCore;
using TasAndJet.Api;
using TasAndJet.Api.Extensions;
using TasAndJet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
//регистрация зависимостей 
builder.Services.AddControllers();
builder.Services.AddOptions();

builder.Services.AddProgramDependencies(builder.Configuration);

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")!));

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.AddMigrationAsync();

}
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();

namespace TasAndJet.Api
{
    public partial class Program;
}

