using System.Data.Common;
using Amazon.S3;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Serilog;
using TasAndJet.Api;
using TasAndJet.Application.Consumers;
using TasAndJet.Infrastructure;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using ILogger = Serilog.ILogger;

namespace TasAndJet.Tests;

public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public IntegrationTestWebFactory()
    {
        WriteLogsToSeq();
    }
    
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
        Console.WriteLine("Запуск контейнера с базой данных...");
        await _dbContainer.StartAsync();
        Console.WriteLine("Контейнер запущен!");

        // Получаем логи контейнера
        var (stdout, stderr) = await _dbContainer.GetLogsAsync();

        Log.Information("[DB Container Log STDOUT] {Stdout}", stdout);
        Log.Error("[DB Container Log STDERR] {Stderr}", stderr);
        

        await _rabbitMqContainer.StartAsync();
        await _minioContainer.StartAsync();
        
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Console.WriteLine("Удаление и создание базы данных...");
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
        Console.WriteLine(_rabbitMqContainer.GetConnectionString());
        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    private async Task InitializeRespawner()
    {
        if (_dbConnection.State != System.Data.ConnectionState.Open)
        {
            await _dbConnection.OpenAsync();
        }
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
        
        services.AddMassTransitTestHarness(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                x.AddConsumer<UserRegisteredEventConsumer>();
                cfg.Host(_rabbitMqContainer.Hostname, _rabbitMqContainer.GetMappedPublicPort(5672), "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            }); 
        }).BuildServiceProvider(true);

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
    
    private void WriteLogsToSeq()
    {
        Log.Logger = new LoggerConfiguration() // Максимальный уровень логирования
            .WriteTo.Console()
            .WriteTo.Debug()
            .WriteTo.Seq("http://localhost:5341") // Отправляем логи в Seq
            .CreateLogger();
    }
}

