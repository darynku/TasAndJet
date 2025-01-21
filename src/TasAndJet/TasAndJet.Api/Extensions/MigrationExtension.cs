using Microsoft.EntityFrameworkCore;
using TasAndJet.Api.Infrastructure;

namespace TasAndJet.Api.Extensions;

public static class MigrationExtension
{
    public static async Task AddMigrationAsync(
        this WebApplication app, 
        CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        
    }
}