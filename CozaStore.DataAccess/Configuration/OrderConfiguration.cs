using CozaStore.Entities.Entities;
using CozaStore.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozaStore.DataAccess.Configuration;

/// <summary>
/// Order entity için Fluent API konfigurasyonları.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Tablo adı
        builder.ToTable("Orders");

        // BaseEntity ortak alanları
        builder.HasKey(o => o.Id);

        builder.Property(o => o.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Sipariş oluşturulma tarihi (UTC)");

        builder.Property(o => o.UpdatedDate)
            .HasComment("Sipariş güncellenme tarihi (UTC)");

        builder.Property(o => o.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Sipariş kaydı aktif mi?");

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Sipariş soft delete flag'i");

        builder.Property(o => o.DeletedDate)
            .HasComment("Sipariş silindiyse silinme tarihi");

        builder.HasIndex(o => o.IsDeleted);
        builder.HasIndex(o => o.IsActive);

        // Siparişe özel alanlar
        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Sipariş numarası");

        builder.Property(o => o.OrderDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("Sipariş tarihi (UTC)");

        builder.Property(o => o.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasComment("Sipariş toplam tutarı");

        builder.Property(o => o.ShippingCost)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0)
            .HasComment("Kargo ücreti");

        builder.Property(o => o.TaxAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0)
            .HasComment("Vergi tutarı");

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>() // Enum değerini string olarak sakla (örn. "Pending")
            .HasMaxLength(50)
            .HasDefaultValue(OrderStatus.Pending)
            .HasComment("Sipariş durumu");

        builder.Property(o => o.PaymentStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(PaymentStatus.Unpaid)
            .HasComment("Ödeme durumu");

        builder.Property(o => o.PaymentMethod)
            .HasConversion<string>()
            .IsRequired(false)
            .HasMaxLength(50)
            .HasComment("Ödeme yöntemi");

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasComment("Siparişi veren kullanıcının Id değeri");

        builder.Property(o => o.ShippingAddressId)
            .IsRequired()
            .HasComment("Teslimat adresi Id'si");

        builder.Property(o => o.BillingAddressId)
            .IsRequired(false)
            .HasComment("Fatura adresi Id'si");

        builder.Property(o => o.Notes)
            .HasMaxLength(1000)
            .HasComment("Sipariş ile ilgili notlar");

        // Index & unique constraint'ler
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique()
            .HasDatabaseName("IX_Orders_OrderNumber");

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.OrderDate);
        builder.HasIndex(o => o.Status);

        // İlişkiler
        builder.HasMany(o => o.OrderDetails)
            .WithOne(od => od.Order)
            .HasForeignKey(od => od.OrderId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_OrderDetails_Orders_OrderId");

        builder.HasOne(o => o.ShippingAddress)
            .WithMany()
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Orders_Addresses_ShippingAddressId");

        builder.HasOne(o => o.BillingAddress)
            .WithMany()
            .HasForeignKey(o => o.BillingAddressId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Orders_Addresses_BillingAddressId");
    }
}
