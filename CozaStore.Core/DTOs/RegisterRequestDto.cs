namespace CozaStore.Core.DTOs;

/// <summary>
/// Kullanıcı kaydı için API'ye gönderilecek veri modeli.
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// Kullanıcı e-posta adresi (UserName olarak kullanılacak).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı parolası.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Parola tekrarı (doğrulama için).
    /// </summary>
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı adı.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Kullanıcı soyadı.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}


