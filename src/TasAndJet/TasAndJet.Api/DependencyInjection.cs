using System.Text;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TasAndJet.Api.Helpers;
using TasAndJet.Application;
using TasAndJet.Application.Clients.Notification;
using TasAndJet.Application.Clients.Notifications;
using TasAndJet.Infrastructure.Options;
using TasAndJet.Infrastructure.Providers;
using TasAndJet.Infrastructure.Providers.Abstract;
using GoogleOptions = TasAndJet.Infrastructure.Options.GoogleOptions;

namespace TasAndJet.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHelpers()
            .AddServices()
            .AddApplicationServices(configuration)
            .AddClients(configuration)
            .AddS3Storage(configuration)
            .AddSwaggerConfiguration()
            .AddAuthConfiguration(configuration)
            .AddMetrics();
        
        return services;
    }

    private static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<CookieHelper>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IJwtProvider, JwtProvider>()
            .AddScoped<IFileProvider, FileProvider>();
        return services;
    }

    private static IServiceCollection AddClients(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmsOptions>(configuration.GetSection("SmsOptions"));
        services.AddSingleton<ISmsSenderService, SmsSenderService>();

        services.AddHttpClient<ISmsClient, SmsClient>();
        
        return services;
    }

    private static IServiceCollection AddS3Storage(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.MinioSection));
    
        var minioOptions = configuration.GetSection(MinioOptions.MinioSection).Get<MinioOptions>()
                           ?? throw new ApplicationException("MinioOptions not found");

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
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TasAndJet API", Version = "v1" });

            
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите JWT токен в формате: Bearer {токен}",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            };

            c.AddSecurityRequirement(securityRequirement);
        });
        return services;
    }

    private static IServiceCollection AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.Configure<GoogleOptions>(configuration.GetSection(GoogleOptions.SectionName));
        
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()
                         ?? throw new ApplicationException("JwtOptions not found");

        var googleOptions = configuration.GetSection(GoogleOptions.SectionName).Get<GoogleOptions>() 
                            ?? throw new ApplicationException("GoogleOptions not found");

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                
            })
            .AddJwtBearer(authenticationScheme: JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    };
                });
        // }).AddCookie(authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme)
        //     .AddGoogle(options =>
        // {
        //     options.ClientId = googleOptions.ClientId;
        //     options.ClientSecret = googleOptions.ClientSecret;
        // }); Web Application != Android app

        services.AddAuthorization();
        return services;
    }

    private static IServiceCollection AddMetrics(this IServiceCollection services)
    {
        const string serviceName = "TasAndJet.Api";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics => metrics
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter())
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.SetDbStatementForText = true;
                    options.SetDbStatementForStoredProcedure = true;
                })
                .AddConsoleExporter()
                /*.AddOtlpExporter(o => o.Endpoint = new Uri("http://jaeger:4317")*/);
        
        return services;
    }
    
}