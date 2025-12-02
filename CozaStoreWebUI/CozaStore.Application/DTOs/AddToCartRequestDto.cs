namespace CozaStore.Application.DTOs;

/// <summary>
/// Sepete ürün eklerken kullanılır.
/// </summary>

public record AddToCartRequestDto(
    Guid ProductId,
    int Quantity
);

