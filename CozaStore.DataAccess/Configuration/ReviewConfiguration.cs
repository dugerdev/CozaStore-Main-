using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// Review entity için Fluent API konfigurasyonları.
/// </summary>
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        // BaseEntity ortak alanları
        builder.Property(r => r.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Yorum oluşturulma tarihi (UTC)");

        builder.Property(r => r.UpdatedDate)
            .HasComment("Yorum güncellenme tarihi (UTC)");

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.DeletedDate);

        builder.HasIndex(r => r.IsDeleted);
        builder.HasIndex(r => r.IsActive);

        // Review özel alanları
        builder.Property(r => r.ProductId)
            .IsRequired()
            .HasComment("Ürün foreign key değeri");

        builder.Property(r => r.UserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasComment("Yorumu yapan kullanıcı Id'si");

        builder.Property(r => r.Title)
            .HasMaxLength(200)
            .HasComment("Yorum başlığı");

        builder.Property(r => r.Comment)
            .HasMaxLength(2000)
            .HasComment("Yorum metni");

        builder.Property(r => r.Rating)
            .IsRequired()
            .HasComment("Puan (1-5)");

        builder.Property(r => r.IsApproved)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Yorum admin tarafından onaylandı mı?");

        // Index'ler
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.IsApproved);

        builder.HasIndex(r => new { r.ProductId, r.IsApproved })
            .HasDatabaseName("IX_Reviews_Product_Approval");

        // İlişkiler
        builder.HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Reviews_Products_ProductId");
    }
}
