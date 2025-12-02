using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// Contact entity için Fluent API konfigurasyonları.
/// </summary>
public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        // BaseEntity ortak alanları
        builder.Property(c => c.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("İletişim mesajı oluşturulma tarihi (UTC)");

        builder.Property(c => c.UpdatedDate)
            .HasComment("İletişim mesajı güncellenme tarihi (UTC)");

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedDate);

        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => c.IsActive);

        // Contact özel alanları
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("Gönderenin email adresi");

        builder.Property(c => c.Message)
            .IsRequired()
            .HasMaxLength(2000)
            .HasComment("Gönderilen mesaj içeriği");

        builder.Property(c => c.IsRead)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Mesaj okundu mu?");

        builder.Property(c => c.ReadDate)
            .HasComment("Mesaj okunma tarihi");

        // Index'ler
        builder.HasIndex(c => c.Email);
        builder.HasIndex(c => c.IsRead);
        builder.HasIndex(c => c.CreatedDate);
    }
}


