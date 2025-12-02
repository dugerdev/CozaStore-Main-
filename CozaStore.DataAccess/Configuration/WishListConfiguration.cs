using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// WishList entity için Fluent API konfigurasyonları.
/// </summary>
public class WishListConfiguration : IEntityTypeConfiguration<WishList>
{
    public void Configure(EntityTypeBuilder<WishList> builder)
    {
        builder.ToTable("WishLists");

        // BaseEntity ortak alanları
        builder.HasKey(w => w.Id);

        builder.Property(w => w.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("İstek listesi kaydı oluşturulma tarihi (UTC)");

        builder.Property(w => w.UpdatedDate)
            .HasComment("İstek listesi kaydı güncellenme tarihi (UTC)");

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Kayıt aktif mi?");

        builder.Property(w => w.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag'i");

        builder.Property(w => w.DeletedDate);

        builder.HasIndex(w => w.IsDeleted);
        builder.HasIndex(w => w.IsActive);

        // WishList özel alanları
        builder.Property(w => w.UserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasComment("Kullanıcı Id değeri");

        builder.Property(w => w.ProductId)
            .IsRequired()
            .HasComment("Ürün Id değeri");

        // Index'ler
        builder.HasIndex(w => w.UserId);
        builder.HasIndex(w => w.ProductId);

        builder.HasIndex(w => new { w.UserId, w.ProductId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_WishLists_User_Product_Unique");

        // İlişkiler
        builder.HasOne(w => w.Product)
            .WithMany(p => p.WishLists)
            .HasForeignKey(w => w.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_WishLists_Products_ProductId");
    }
}
