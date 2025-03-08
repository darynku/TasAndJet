using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Infrastructure.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> builder)
    {
        builder.ToTable("UserSubscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.StripeSubscriptionId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate);

        builder.HasOne<User>()
            .WithOne(u => u.UserSubscription)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
