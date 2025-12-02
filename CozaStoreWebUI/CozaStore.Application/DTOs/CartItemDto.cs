namespace CozaStore.Application.DTOs;

/// <summary>
/// Sepetteki ürün bilgilerini taşır.
/// SubTotal = ProductPrice * Quantity (otomatik hesaplanır).
/// </summary>

public record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal ProductPrice,
    string? ProductImageUrl,
    int Quantity,
    decimal SubTotal
);

