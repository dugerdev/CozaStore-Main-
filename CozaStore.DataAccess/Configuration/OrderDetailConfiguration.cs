using CozaStore.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// OrderDetail entity için Fluent API konfigurasyonları.
/// </summary>
public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");

        // BaseEntity ortak alanları
        builder.HasKey(od => od.Id);

        builder.Property(od => od.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Sipariş detayı oluşturulma tarihi (UTC)");

        builder.Property(od => od.UpdatedDate)
            .HasComment("Sipariş detayı güncellenme tarihi (UTC)");

        builder.Property(od => od.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Kayıt aktif mi?");

        builder.Property(od => od.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag'i");

        builder.Property(od => od.DeletedDate);

        builder.HasIndex(od => od.IsDeleted);
        builder.HasIndex(od => od.IsActive);

        // OrderDetail özel alanları
        builder.Property(od => od.OrderId)
            .IsRequired()
            .HasComment("Sipariş foreign key değeri");

        builder.Property(od => od.ProductId)
            .IsRequired()
            .HasComment("Ürün foreign key değeri");

        builder.Property(od => od.ProductName)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Sipariş anındaki ürün adı");

        builder.Property(od => od.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasComment("Sipariş anındaki birim fiyat");

        builder.Property(od => od.Quantity)
            .IsRequired()
            .HasComment("Sipariş edilen miktar");

        builder.Property(od => od.SubTotal)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasComment("Satır toplamı (UnitPrice * Quantity)");

        // Index'ler
        builder.HasIndex(od => od.OrderId);
        builder.HasIndex(od => od.ProductId);

        // İlişkiler
        builder.HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_OrderDetails_Orders_OrderId");

        builder.HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_OrderDetails_Products_ProductId");
    }
}
