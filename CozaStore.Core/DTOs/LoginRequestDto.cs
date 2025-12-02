namespace CozaStore.Core.DTOs;

/// <summary>
/// Kullanıcı girişi için API'ye gönderilecek veri modeli.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Kullanıcı e-posta adresi (UserName olarak kullanılacak).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı parolası.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}


