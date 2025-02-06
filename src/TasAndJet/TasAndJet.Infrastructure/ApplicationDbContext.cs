using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TasAndJet.Domain.Entities.Account;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Reviews;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Infrastructure;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<Order> Orders => Set<Order>(); 
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Review> Reviews => Set<Review>();
    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Default"));
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Role>()
            .HasData(
            new Role(1, "Admin"),
            new Role(2, "User"),
            new Role(3, "Driver"),
            new Role(4, "Customer"));

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.Information));
}