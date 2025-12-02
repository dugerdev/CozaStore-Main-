using CozaStore.Entities.Entities;
using CozaStore.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// Address entity için Fluent API konfigurasyonları.
/// </summary>
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");

        // BaseEntity ortak alanları
        builder.HasKey(a => a.Id);

        builder.Property(a => a.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Adres oluşturulma tarihi (UTC)");

        builder.Property(a => a.UpdatedDate)
            .HasComment("Adres güncellenme tarihi (UTC)");

        builder.Property(a => a.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(a => a.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.DeletedDate);

        builder.HasIndex(a => a.IsDeleted);
        builder.HasIndex(a => a.IsActive);

        // Adrese özel alanlar
        builder.Property(a => a.UserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasComment("Kullanıcı Id değeri");

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Adres başlığı");

        builder.Property(a => a.AddressLine1)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Adres satırı 1");

        builder.Property(a => a.AddressLine2)
            .HasMaxLength(200)
            .HasComment("Adres satırı 2");

        builder.Property(a => a.City)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Şehir");

        builder.Property(a => a.District)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("İlçe");

        builder.Property(a => a.PostalCode)
            .IsRequired(false)
            .HasMaxLength(20)
            .HasComment("Posta kodu");

        builder.Property(a => a.Country)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("Turkey")
            .HasComment("Ülke");

        builder.Property(a => a.AddressType)
            .HasConversion<string>()
            .IsRequired(false)
            .HasMaxLength(50)
            .HasComment("Adres tipi (Shipping / Billing)");

        builder.Property(a => a.IsDefault)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Varsayılan adres mi?");

        // Index'ler
        builder.HasIndex(a => a.UserId);

        builder.HasIndex(a => new { a.UserId, a.IsDefault })
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_Addresses_User_Default");
    }
}
