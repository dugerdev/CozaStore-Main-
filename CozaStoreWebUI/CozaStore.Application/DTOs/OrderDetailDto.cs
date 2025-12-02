namespace CozaStore.Application.DTOs;

/// <summary>
/// Siparişteki her bir ürünün bilgilerini taşır.
/// </summary>

public record OrderDetailDto(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal SubTotal
);

