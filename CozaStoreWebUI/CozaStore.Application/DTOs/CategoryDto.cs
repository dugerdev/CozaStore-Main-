namespace CozaStore.Application.DTOs;

/// <summary>
/// Kategori bilgilerini taşır.
/// API'den gelen format ile uyumlu olmalı.
/// Record type kullanılır (immutable, value equality).
/// </summary>

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    DateTime CreatedDate,  // API'de var
    bool IsActive  // API'de var
);

