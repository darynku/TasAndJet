using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasAndJet.Domain.Entities.Orders;
using TasAndJet.Domain.Entities.Services;

namespace TasAndJet.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.PickupAddress)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(o => o.DestinationAddress)
            .IsRequired(false)
            .HasMaxLength(255);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();        
        
        builder.Property(o => o.City)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.ImageKeys)
            .IsRequired(false);

    }
}

