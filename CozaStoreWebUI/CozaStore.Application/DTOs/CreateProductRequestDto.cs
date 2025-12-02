namespace CozaStore.Application.DTOs;

/// <summary>
/// Yeni ürün oluşturma isteği
/// </summary>
public record CreateProductRequestDto(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    string? ImageUrl,
    Guid CategoryId,
    bool IsActive = true
);


