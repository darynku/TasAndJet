using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TasAndJet.Application.Applications.Services.Accounts.Google;
using TasAndJet.Application.Applications.Services.Accounts.UploadFile;
using TasAndJet.Application.Consumers;
using TasAndJet.Application.Hubs;

namespace TasAndJet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddServices()
            .AddMediator()
            .AddValidators()
            .AddCache()
            .AddMessageBus(configuration)
            .AddSignalr();
        
        return services;
    }
    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IUploadFileService, UploadFileService>()
            .AddScoped<IGoogleAuthService, GoogleAuthService>();

        return services;
    }
    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); });

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }
    
    private static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.AddConsumer<UserRegisteredEventConsumer>();
            
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(configuration["RabbitMqOptions:Host"]!), h =>
                {
                    h.Username(configuration["RabbitMqOptions:UserName"]!);
                    h.Password(configuration["RabbitMqOptions:Password"]!);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        return services;
    }
    private static IServiceCollection AddSignalr(this IServiceCollection services)
    {
        services
            .AddSignalR()
            .AddJsonProtocol()
            .AddHubOptions<NotificationHub>(options =>
            {
                options.EnableDetailedErrors = true;
            });
        
        return services;
    }
}