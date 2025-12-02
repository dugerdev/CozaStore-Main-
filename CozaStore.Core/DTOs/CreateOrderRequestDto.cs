using CozaStore.Entities.Enums;

namespace CozaStore.Core.DTOs;

/// <summary>
/// Yeni sipariş oluşturma isteği için DTO.
/// </summary>
public class CreateOrderRequestDto
{
    public Guid ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public decimal ShippingCost { get; set; } = 0;
    public decimal TaxAmount { get; set; } = 0;
    public string? Notes { get; set; }
    public List<OrderDetailItemDto> OrderDetails { get; set; } = new();
}

/// <summary>
/// Sipariş detay öğesi (ürün ve miktar).
/// </summary>
public class OrderDetailItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

