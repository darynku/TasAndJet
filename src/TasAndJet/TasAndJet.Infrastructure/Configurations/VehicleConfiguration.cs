using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Infrastructure.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);
        
        builder.Property(v => v.VehicleType).IsRequired();
        
        builder.Property(v => v.Mark).IsRequired();
        
        builder.Property(v => v.Capacity).IsRequired();
        
        builder.Property(v => v.PhotoUrl).IsRequired(false);
    }

}