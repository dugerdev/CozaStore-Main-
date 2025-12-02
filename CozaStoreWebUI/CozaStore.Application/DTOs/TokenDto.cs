namespace CozaStore.Application.DTOs;

/// <summary>
/// JWT authentication token bilgilerini taşır.
/// </summary>

public record TokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);

