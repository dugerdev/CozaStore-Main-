using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// Product entity için Fluent API konfigurasyonları.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // ------------------------------------------------------------
        // Tablo adı
        // ------------------------------------------------------------
        builder.ToTable("Products");

        // ------------------------------------------------------------
        // BaseEntity ortak alanları
        // ------------------------------------------------------------
        builder.HasKey(p => p.Id); // Primary key

        builder.Property(p => p.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Kaydın oluşturulma tarihi (UTC)");

        builder.Property(p => p.UpdatedDate)
            .HasComment("Kaydın son güncellenme tarihi (UTC)");

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Kayıt aktif mi?");

        builder.Property(p => p.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag'i");

        builder.Property(p => p.DeletedDate)
            .HasComment("Kayıt silindiyse silinme tarihi (UTC)");

        builder.HasIndex(p => p.IsDeleted);
        builder.HasIndex(p => p.IsActive);

        // ------------------------------------------------------------
        // Ürün özel alanları
        // ------------------------------------------------------------
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Ürün adı");

        builder.Property(p => p.Description)
            .HasMaxLength(2000)
            .HasComment("Ürün açıklaması");

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasComment("Ürün satış fiyatı");

        builder.Property(p => p.DiscountPrice)
            .HasColumnType("decimal(18,2)")
            .HasComment("Ürünün indirimli fiyatı");

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(2000)
            .HasComment("Ürün görseli URL'i");

        builder.Property(p => p.SKU)
            .HasMaxLength(50)
            .HasComment("Stock Keeping Unit (stok kodu)");

        builder.Property(p => p.StockQuantity)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Stok adedi");

        builder.Property(p => p.IsInStock)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Ürün stokta mı?");

        builder.Property(p => p.CategoryId)
            .IsRequired()
            .HasComment("Kategori foreign key değeri");

        // ------------------------------------------------------------
        // Index & Unique constraint'ler
        // ------------------------------------------------------------
        builder.HasIndex(p => p.CategoryId);

        builder.HasIndex(p => p.SKU)
            .IsUnique()
            .HasFilter("[SKU] IS NOT NULL")
            .HasDatabaseName("IX_Products_SKU_NotNull");

        // ------------------------------------------------------------
        // İlişkiler
        // ------------------------------------------------------------
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Products_Categories_CategoryId");

        builder.HasMany(p => p.OrderDetails)
            .WithOne(od => od.Product)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_OrderDetails_Products_ProductId");

        builder.HasMany(p => p.CartItems)
            .WithOne(ci => ci.Product)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CartItems_Products_ProductId");

        builder.HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Reviews_Products_ProductId");

        builder.HasMany(p => p.WishLists)
            .WithOne(w => w.Product)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_WishLists_Products_ProductId");
    }
}
