namespace CozaStore.Application.DTOs;

/// <summary>
/// Sepetteki ürün miktarını güncellerken kullanılır.
/// </summary>
public record UpdateCartQuantityRequestDto(
    int Quantity
);


