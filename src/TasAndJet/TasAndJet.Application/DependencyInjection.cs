using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TasAndJet.Application.Consumers;
using TasAndJet.Infrastructure.Options;

namespace TasAndJet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMediator()
            .AddValidators()
            .AddCache()
            .AddMessageBus(configuration);
        
        return services;
    }
    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        return services;
    }
    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(config => { config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); });

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
    
    private static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqOptions = configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() 
                              ?? throw new ApplicationException(nameof(RabbitMqOptions));
        
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));
        
        services.AddMassTransit(configure =>
        {
            configure.SetKebabCaseEndpointNameFormatter();

            configure.AddConsumer<UserRegisteredEventConsumer>();
            
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqOptions.Host), h =>
                {
                    h.Username(rabbitMqOptions.Username);
                    h.Password(rabbitMqOptions.Password);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}