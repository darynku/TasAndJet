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
            .ValueGeneratedNever(); // Указываем, что Id задается вручную

        builder.Property(user => user.FirstName)
            .IsRequired()
            .HasMaxLength(100); // Ограничение длины строки

        builder.Property(user => user.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(user => user.PasswordHash)
            .IsRequired();

        builder.Property(user => user.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(user => user.Region)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(user => user.Address)
            .IsRequired()
            .HasMaxLength(300);

        // Клиентские заказы
        builder.HasMany(u => u.ClientOrders)
            .WithOne(o => o.Client)
            .HasForeignKey(o => o.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        // Водительские заказы
        builder.HasMany(u => u.DriverOrders)
            .WithOne(o => o.Driver)
            .HasForeignKey(o => o.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с ролью (Role)
        builder
            .HasOne(user => user.Role)
            .WithMany() // Связь "один ко многим" (если Role содержит коллекцию Users, замените на .WithMany(role => role.Users))
            .HasForeignKey("RoleId") // Внешний ключ RoleId
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); // Указываем поведение при удалении

        // Индексы
        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.PhoneNumber).IsUnique();
    }
}