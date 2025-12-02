using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// BlogPost entity için Fluent API konfigurasyonları.
/// </summary>
public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        // Tablo adı
        builder.ToTable("BlogPosts");

        // BaseEntity ortak alanları
        builder.HasKey(b => b.Id);

        builder.Property(b => b.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Kaydın oluşturulma tarihi (UTC)");

        builder.Property(b => b.UpdatedDate)
            .HasComment("Kaydın son güncellenme tarihi (UTC)");

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Kayıt aktif mi?");

        builder.Property(b => b.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag'i");

        builder.Property(b => b.DeletedDate)
            .HasComment("Kayıt silindiyse silinme tarihi (UTC)");

        builder.HasIndex(b => b.IsDeleted);
        builder.HasIndex(b => b.IsActive);
        builder.HasIndex(b => b.IsPublished);

        // BlogPost özel alanları
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Blog gönderisi başlığı");

        builder.Property(b => b.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)")
            .HasComment("Blog gönderisi içeriği");

        builder.Property(b => b.ImageUrl)
            .HasMaxLength(2000)
            .HasComment("Blog gönderisi görseli URL'i");

        builder.Property(b => b.IsPublished)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Blog gönderisi yayınlandı mı?");

        builder.Property(b => b.AuthorId)
            .IsRequired()
            .HasComment("Blog gönderisini oluşturan kullanıcı ID'si");

        // İlişki konfigurasyonu
        builder.HasOne(b => b.Author)
            .WithMany()
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
