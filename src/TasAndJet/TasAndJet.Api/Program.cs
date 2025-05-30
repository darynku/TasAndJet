using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Stripe;
using TasAndJet.Api;
using TasAndJet.Api.Extensions;
using TasAndJet.Application.Hubs;
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

// builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddStackExchangeRedisCache(action =>
{
    action.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddResponseCaching();
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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub", options =>
{
    options.Transports = HttpTransportType.WebSockets;
});

app.UseHttpsRedirection();

app.MapControllers();
app.Run();

namespace TasAndJet.Api
{
    public partial class Program;
}

