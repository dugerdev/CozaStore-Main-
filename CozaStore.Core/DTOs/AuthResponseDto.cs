namespace CozaStore.Core.DTOs;

/// <summary>
/// Authentication işlemlerinden dönen yanıt modeli (JWT token ve kullanıcı bilgileri).
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// JWT token (API'ye isteklerde Authorization header'ında kullanılacak).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Token'ın geçerlilik süresi (dakika cinsinden).
    /// </summary>
    public int ExpirationInMinutes { get; set; }

    /// <summary>
    /// Kullanıcı e-posta adresi.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı adı.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Kullanıcı soyadı.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Kullanıcının rollerini içeren liste.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}


