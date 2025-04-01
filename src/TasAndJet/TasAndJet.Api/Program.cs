using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TasAndJet.Api;
using TasAndJet.Api.Extensions;
using TasAndJet.Infrastructure;
using TasAndJet.Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


builder.Services.AddEndpointsApiExplorer();
//регистрация зависимостей 
builder.Services.AddControllers();
builder.Services.AddOptions();

builder.Services.AddProgramDependencies(builder.Configuration);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")!));

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("StripeOptions"));

var stripeOptions = builder.Configuration.GetSection("StripeOptions").Get<StripeOptions>()
                    ?? throw new Exception("StripeOptions not found in configuration");

StripeConfiguration.ApiKey = stripeOptions.SecretKey;

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

