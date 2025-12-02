using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// CartItem entity için Fluent API konfigurasyonları.
/// </summary>
public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        // BaseEntity ortak alanları
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Sepet kaydı oluşturulma tarihi (UTC)");

        builder.Property(ci => ci.UpdatedDate)
            .HasComment("Sepet kaydı güncellenme tarihi (UTC)");

        builder.Property(ci => ci.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Sepet kaydı aktif mi?");

        builder.Property(ci => ci.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag'i");

        builder.Property(ci => ci.DeletedDate);

        builder.HasIndex(ci => ci.IsDeleted);
        builder.HasIndex(ci => ci.IsActive);

        // CartItem özel alanları
        builder.Property(ci => ci.UserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasComment("Kullanıcı Id değeri");

        builder.Property(ci => ci.ProductId)
            .IsRequired()
            .HasComment("Ürün Id değeri");

        builder.Property(ci => ci.Quantity)
            .IsRequired()
            .HasDefaultValue(1)
            .HasComment("Sepete eklenen ürün adedi");

        // Index'ler
        builder.HasIndex(ci => ci.UserId);
        builder.HasIndex(ci => ci.ProductId);

        builder.HasIndex(ci => new { ci.UserId, ci.ProductId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_CartItems_User_Product_Unique");

        // İlişkiler
        builder.HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_CartItems_Products_ProductId");
    }
}
