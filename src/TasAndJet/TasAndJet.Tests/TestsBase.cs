
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;

namespace TasAndJet.Tests;

public abstract class TestsBase : IClassFixture<IntegrationTestWebFactory>, IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly IntegrationTestWebFactory Factory;
    protected readonly ApplicationDbContext Context;
    protected readonly IMediator Mediator;
    private readonly Func<Task> _resetDatabase;

    public TestsBase(IntegrationTestWebFactory factory)
    {
        _resetDatabase = factory.ResetDatabaseAsync;
        _scope = factory.Services.CreateScope();
        
        Mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        Context = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        Factory = factory;
        
    }
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _scope.Dispose();
        return Task.CompletedTask;
    }

    protected async Task SeedRoles()
    {
        var roles = new List<Role>
        {
            new Role(1, "Admin"),
            new Role(2, "User"),
            new Role(3, "Driver")
        };

        if (!Context.Roles.Any())
        {
            Context.Roles.AddRange(roles);
            await Context.SaveChangesAsync();
        }
    }
}