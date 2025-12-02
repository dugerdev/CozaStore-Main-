namespace CozaStore.Application.DTOs;

/// <summary>
/// İletişim mesajı bilgilerini taşır.
/// </summary>
public record ContactDto(
    Guid Id,
    string Email,
    string Message,
    bool IsRead,
    DateTime? ReadDate,
    DateTime CreatedDate
);


