using System.Data.Common;
using Amazon.S3;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using TasAndJet.Api;
using TasAndJet.Infrastructure;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace TasAndJet.Tests;

public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("test")
        .WithUsername("postgres")
        .WithPassword("123")
        .Build();

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:3-management")
        .WithUsername("guest")
        .WithPassword("guest")
        .Build();
    
    private readonly MinioContainer _minioContainer = new MinioBuilder()
        .WithImage("minio/minio")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();


    private Respawner _respawner = default!;
    private DbConnection _dbConnection = default!;
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        await _minioContainer.StartAsync();
        
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await InitializeRespawner();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();

        await _rabbitMqContainer.StopAsync();
        await _rabbitMqContainer.DisposeAsync();

        await _minioContainer.StopAsync();
        await _minioContainer.DisposeAsync();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:Database", _dbContainer.GetConnectionString() },
                { "RabbitMqOptions:Host", _rabbitMqContainer.GetConnectionString() },
                { "RabbitMqOptions:Username", "guest" },
                { "RabbitMqOptions:Password", "guest" },
            });
        });

        Console.WriteLine(_dbContainer.GetConnectionString());
        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres, SchemasToInclude = ["public"], });
    }
    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(_dbContainer.GetConnectionString()));
        

        services.RemoveAll<IAmazonS3>();

        ushort port = _minioContainer.GetMappedPublicPort(9000);

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = $"http://{_minioContainer.Hostname}:{port}", UseHttp = true, ForcePathStyle = true
            };

            return new AmazonS3Client("minioadmin", "minioadmin", config);
        });
    }
}