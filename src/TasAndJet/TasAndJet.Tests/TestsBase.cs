
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Infrastructure;

namespace TasAndJet.Tests;

public abstract class TestsBase : IClassFixture<IntegrationTestWebFactory>, IAsyncLifetime
{
    protected readonly IServiceScope Scope;
    protected readonly IntegrationTestWebFactory Factory;
    protected readonly ApplicationDbContext Context;
    protected readonly IMediator Mediator;
    private readonly Func<Task> _resetDatabase;

    public TestsBase(IntegrationTestWebFactory factory)
    {
        _resetDatabase = factory.ResetDatabaseAsync;
        Scope = factory.Services.CreateScope();
        
        Mediator = Scope.ServiceProvider.GetRequiredService<IMediator>();
        Context = Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        Factory = factory;
        
    }
    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        Scope.Dispose();
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
    
    public async Task<User> SeedUser(string email, string password, int roleId)
    {
        await SeedRoles();

        var role = await Context.Roles.FirstOrDefaultAsync(role => role.Id == roleId)
                   ?? throw new Exception($"Role with id {roleId} not found");
        
        var user = User.CreateUser(
            Guid.NewGuid(),
            "Test",
            "User",
            email,
            BCrypt.Net.BCrypt.EnhancedHashPassword(password),
            "+1234567890",
            "Test Region",
            "123 Test St",
            role
        );
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();
        return user;
    }
}