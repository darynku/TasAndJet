﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasAndJet.Api.Entities.Account;
using TasAndJet.Api.Entities.Orders;
using TasAndJet.Api.Entities.Services;

namespace TasAndJet.Api.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.PickupAddress)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.DestinationAddress)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne<Service>(o => o.Service)
            .WithMany()
            .HasForeignKey("ServiceId") 
            .OnDelete(DeleteBehavior.Restrict);
    }
}

