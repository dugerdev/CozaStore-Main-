namespace CozaStore.Application.DTOs;

/// <summary>
/// Ürün güncelleme isteği
/// </summary>
public record UpdateProductRequestDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId,
    bool IsActive
);


