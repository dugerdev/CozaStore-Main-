namespace CozaStore.Application.DTOs;

/// <summary>
/// Kullanıcı giriş bilgilerini taşır.
/// </summary>

public record LoginRequestDto(
    string Email,
    string Password
);

