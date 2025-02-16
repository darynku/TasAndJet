using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TasAndJet.Domain.Entities.Account;

namespace TasAndJet.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Указание имени таблицы
        builder.ToTable("Users");

        // Первичный ключ
        builder.HasKey(user => user.Id);

        // Свойства
        builder.Property(user => user.Id)
            .IsRequired()
            .ValueGeneratedNever(); // Id задается вручную

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Email)
            .HasMaxLength(200);

        builder.Property(user => user.PasswordHash);

        builder.Property(user => user.PhoneNumber)
            .HasMaxLength(15);

        builder.Property(user => user.Region)
            .HasMaxLength(100);

        builder.Property(user => user.Address)
            .HasMaxLength(300);

        // 🔹 Stripe данные
        builder.Property(user => user.StripeCustomerId)
            .HasMaxLength(50); // Ограничение на длину идентификатора

        builder.Property(user => user.StripePaymentMethodId)
            .HasMaxLength(50);

        builder.Property(user => user.StripeAccountId)
            .HasMaxLength(50);

        // 🔹 Google данные
        builder.Property(user => user.GoogleId)
            .HasMaxLength(50);

        // 🔹 Подтверждение телефона
        builder.Property(user => user.PhoneConfirmed);

        // 🔹 Клиентские заказы
        builder.HasMany(u => u.ClientOrders)
            .WithOne(o => o.Client)
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // 🔹 Водительские заказы
        builder.HasMany(u => u.DriverOrders)
            .WithOne(o => o.Driver)
            .HasForeignKey(o => o.DriverId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // 🔹 Связь с ролью
        builder.HasOne(user => user.Role)
            .WithMany() // Если Role содержит Users, заменить на .WithMany(role => role.Users)
            .HasForeignKey("RoleId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // 🔹 Индексы (уникальность)
        /*builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.PhoneNumber).IsUnique();*/
    }
}
