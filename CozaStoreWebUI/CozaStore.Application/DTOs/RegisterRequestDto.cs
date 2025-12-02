namespace CozaStore.Application.DTOs;

/// <summary>
/// Yeni kullanıcı kayıt bilgilerini taşır.
/// </summary>

public record RegisterRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword
);

