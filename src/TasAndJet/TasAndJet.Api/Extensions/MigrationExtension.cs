using Microsoft.EntityFrameworkCore;
using TasAndJet.Infrastructure;

namespace TasAndJet.Api.Extensions;

public static class MigrationExtension
{
    public static async Task AddMigrationAsync(
        this WebApplication app, 
        CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await dbContext.Database.MigrateAsync(cancellationToken: cancellationToken);
        
    }
}