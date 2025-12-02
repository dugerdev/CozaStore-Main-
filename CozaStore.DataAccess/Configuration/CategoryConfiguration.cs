using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// Category entity için Fluent API konfigurasyonları.
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Tablo adı
        builder.ToTable("Categories");

        // BaseEntity ortak alanları
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Kaydın oluşturulma tarihi (UTC)");

        builder.Property(c => c.UpdatedDate)
            .HasComment("Kaydın son güncellenme tarihi (UTC)");

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Kayıt aktif mi?");

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag'i");

        builder.Property(c => c.DeletedDate)
            .HasComment("Kayıt silindiyse silinme tarihi (UTC)");

        builder.HasIndex(c => c.IsDeleted);
        builder.HasIndex(c => c.IsActive);

        // Category özel alanları
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Kategori adı");

        builder.Property(c => c.Description)
            .HasMaxLength(500)
            .HasComment("Kategori açıklaması");

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(2000)
            .HasComment("Kategori görseli URL'i");

        // İlişki konfigurasyonu ProductConfiguration içerisinde yönetiliyor.
    }
}
