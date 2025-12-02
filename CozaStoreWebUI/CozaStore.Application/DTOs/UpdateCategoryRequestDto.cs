namespace CozaStore.Application.DTOs;

/// <summary>
/// Kategori güncelleme isteği
/// </summary>
public record UpdateCategoryRequestDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    bool IsActive
);


