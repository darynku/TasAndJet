using Microsoft.EntityFrameworkCore;
using TasAndJet.Api.Entities.Account;
using TasAndJet.Api.Entities.Orders;
using TasAndJet.Api.Entities.Services;

namespace TasAndJet.Api.Infrastructure;

public class ApplicationDbContext(IConfiguration configuration) : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();
    public DbSet<Order> Orders => Set<Order>(); 
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Service> Services => Set<Service>();
    
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
        LoggerFactory.Create(builder => builder.AddConsole());
}