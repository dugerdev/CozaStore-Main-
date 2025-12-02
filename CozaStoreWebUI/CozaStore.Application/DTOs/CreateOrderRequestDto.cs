using CozaStore.Application.Enums;

namespace CozaStore.Application.DTOs;


/// <summary>
/// Yeni sipariş oluştururken kullanılır.
/// OrderDetails: Sipariş edilen ürünler listesi.
/// </summary>

public record CreateOrderRequestDto(
    Guid ShippingAddressId,
    Guid? BillingAddressId,
    PaymentMethod PaymentMethod,
    decimal ShippingCost,
    decimal TaxAmount,
    string? Notes,
    List<OrderItemRequestDto> OrderDetails
);


/// <summary>
/// Sipariş içindeki her bir ürün bilgisi.
/// </summary>

public record OrderItemRequestDto(
    Guid ProductId,
    int Quantity
);

