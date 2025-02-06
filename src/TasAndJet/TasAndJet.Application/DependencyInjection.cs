using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TasAndJet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddMediator()
            .AddValidators()
            .AddCache();
        
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

}