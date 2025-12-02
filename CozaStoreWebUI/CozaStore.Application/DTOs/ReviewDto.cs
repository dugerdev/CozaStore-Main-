namespace CozaStore.Application.DTOs;

/// <summary>
/// Ürün yorumu bilgilerini taşır.
/// Rating: 1-5 arası puan.
/// </summary>

public record ReviewDto(
    Guid Id,
    Guid ProductId,
    string UserId,
    string? Title,
    string? Comment,
    int Rating,
    bool IsApproved,
    DateTime CreatedDate
);

