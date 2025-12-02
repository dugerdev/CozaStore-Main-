namespace CozaStore.Application.DTOs;

/// <summary>
/// İstek listesindeki ürün bilgilerini taşır.
/// </summary>

public record WishListDto(
    Guid Id,
    string UserId,
    Guid ProductId,
    string ProductName,
    decimal ProductPrice,
    string? ProductImageUrl
);

