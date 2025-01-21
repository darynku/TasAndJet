using System.Reflection;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TasAndJet.Api.Infrastructure;
using TasAndJet.Api.Infrastructure.Providers;
using TasAndJet.Api.Infrastructure.Repositories;

namespace TasAndJet.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(this IServiceCollection services)
    {
        services
            .AddDataAccess()
            .AddServices()
            .AddMediator()
            .AddValidators()
            .AddCache();
        return services;
    }

    private static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IRepository<>), typeof(EntityFrameworkRepository<>))
            .AddScoped<ApplicationDbContext>();
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        return services;
    }

    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        return services;
    }

}