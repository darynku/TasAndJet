﻿using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using TasAndJet.Api.Infrastructure;
using TasAndJet.Api.Infrastructure.Options;
using TasAndJet.Api.Infrastructure.Providers;
using TasAndJet.Api.Infrastructure.Providers.Abstract;
using TasAndJet.Api.Infrastructure.Repositories;

namespace TasAndJet.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDataAccess()
            .AddServices()
            .AddMediator()
            .AddValidators()
            .AddCache()
            .AddS3Storage(configuration)
            .AddSwaggerConfiguration()
            .AddJwtConfiguration(configuration);
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
        services
            .AddScoped<IJwtProvider, JwtProvider>()
            .AddScoped<IFileProvider, FileProvider>();
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

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        return services;
    }

    private static IServiceCollection AddS3Storage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.MinioSection));
    
        var minioOptions = configuration.GetSection(MinioOptions.MinioSection).Get<MinioOptions>()
                           ?? throw new ApplicationException("MinioOptions not found");

        services.AddMinio(config =>
        {
            config
                .WithEndpoint(minioOptions.Endpoint)
                .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
                .WithSSL(minioOptions.WithSsl)
                .Build();
        });

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = $"http://{minioOptions.Endpoint}",
                UseHttp = minioOptions.WithSsl,
                ForcePathStyle = true
            };

            return new AmazonS3Client(minioOptions.AccessKey, minioOptions.SecretKey, config);
        });

        return services;
    }

    private static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "My API", Version = "v1",
            });
            c.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme, Id = "Bearer",
                        },
                    },
                    []
                },
            });
        });
        return services;
    }

    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
                         ?? throw new ApplicationException("JwtOptions not found");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(authenticationScheme: JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            };
        });
        services.AddAuthorization();
        return services;
    }
}