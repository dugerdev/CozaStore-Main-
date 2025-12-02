namespace CozaStore.Application.DTOs;

/// <summary>
/// Yeni kategori oluşturma isteği
/// </summary>
public record CreateCategoryRequestDto(
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsActive = true
);


