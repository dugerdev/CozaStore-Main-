using CozaStore.Entities.Enums;

namespace CozaStore.Core.DTOs;

/// <summary>
/// Sipariş bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public Guid ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}


