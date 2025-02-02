using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasAndJet.Api.Entities.Services;

namespace TasAndJet.Api.Infrastructure.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Cost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.ServiceType)
            .HasConversion<string>()
            .IsRequired();
    }
}
