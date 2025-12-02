namespace CozaStore.Application.DTOs;

/// <summary>
/// Ürün bilgilerini taşır.
/// API'den gelen format ile uyumlu olmalı.
/// </summary>

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,  // API'de Stock olarak geliyor
    string? ImageUrl,
    Guid CategoryId,
    string? CategoryName,
    DateTime CreatedDate,  // API'de var
    bool IsActive
);

