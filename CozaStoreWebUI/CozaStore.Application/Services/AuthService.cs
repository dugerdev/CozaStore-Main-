using System.Text;
using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;


/// <summary>
/// Kimlik doğrulama işlemleri için API ile iletişim kurar.
/// Login, Register, Token yönetimi.
/// </summary>

public class AuthService
{
    
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    

    
    public AuthService(HttpClient httpClient)
    {
        // BaseAddress Program.cs'de HttpClient yapılandırması ile set ediliyor
        _httpClient = httpClient;
        
        // JSON deserialization için case-insensitive options
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
    

    
    /// <summary>
    /// Kullanıcı kaydı (Register)
    /// </summary>
    /// <param name="request">Kayıt bilgileri (Ad, Soyad, Email, Şifre)</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> RegisterAsync(RegisterRequestDto request)
    {
        try
        {
            // DTO'yu JSON formatına çevir
            var json = JsonSerializer.Serialize(request);
            
            // HTTP içeriği oluştur (UTF8, JSON formatında)
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // API'ye POST isteği gönder: POST api/auth/register
            var response = await _httpClient.PostAsync("register", content);
            
            // Yanıtı oku
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // HTTP status code kontrolü
            if (!response.IsSuccessStatusCode)
            {
                // API'den gelen hata mesajını parse et
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<Result>(responseContent, _jsonOptions);
                        if (errorResult != null && !string.IsNullOrEmpty(errorResult.ErrorMessage))
                        {
                            return Result.Failure(errorResult.ErrorMessage);
                        }
                    }
                    catch
                    {
                        // JSON parse hatası durumunda genel mesaj göster
                    }
                }
                
                // Genel hata mesajı
                return Result.Failure("Kayıt başarısız. Lütfen bilgilerinizi kontrol edip tekrar deneyin.");
            }
            
            // Boş yanıt kontrolü
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return Result.Failure("Kayıt başarısız. Lütfen tekrar deneyin.");
            }
            
            // JSON'dan Result'a dönüştür
            try
            {
                return JsonSerializer.Deserialize<Result>(responseContent, _jsonOptions);
            }
            catch (JsonException)
            {
                return Result.Failure("Kayıt başarısız. Lütfen tekrar deneyin.");
            }
        }
        catch (Exception ex)
        {
            return Result.Failure($"Kayıt başarısız: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Kullanıcı girişi (Login)
    /// </summary>
    /// <param name="request">Email ve Şifre</param>
    /// <returns>Token bilgileri (AccessToken, RefreshToken)</returns>
    
    public async Task<Result<TokenDto>?> LoginAsync(LoginRequestDto request)
    {
        try
        {
            // Request'i JSON'a çevir
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // POST api/auth/login
            var response = await _httpClient.PostAsync("login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // HTTP status code kontrolü
            if (!response.IsSuccessStatusCode)
            {
                // API'den gelen hata mesajını parse et
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<Result>(responseContent, _jsonOptions);
                        if (errorResult != null && !string.IsNullOrEmpty(errorResult.ErrorMessage))
                        {
                            return Result<TokenDto>.Failure(errorResult.ErrorMessage);
                        }
                    }
                    catch
                    {
                        // JSON parse hatası durumunda genel mesaj göster
                    }
                }
                
                // Genel hata mesajı
                return Result<TokenDto>.Failure("Email veya şifre hatalı.");
            }
            
            // Boş yanıt kontrolü
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return Result<TokenDto>.Failure("Giriş başarısız. Lütfen tekrar deneyin.");
            }
            
            // JSON'dan Result'a dönüştür
            try
            {
                return JsonSerializer.Deserialize<Result<TokenDto>>(responseContent, _jsonOptions);
            }
            catch (JsonException)
            {
                return Result<TokenDto>.Failure("Giriş başarısız. Lütfen tekrar deneyin.");
            }
        }
        catch (Exception ex)
        {
            return Result<TokenDto>.Failure($"Giriş başarısız: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Çıkış yap (Logout)
    /// Token'ı geçersiz kıl
    /// </summary>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> LogoutAsync()
    {
        try
        {
            // POST api/auth/logout
            var response = await _httpClient.PostAsync("logout", null);
            var content = await response.Content.ReadAsStringAsync();
            
            // Boş yanıt kontrolü
            if (string.IsNullOrWhiteSpace(content))
            {
                return Result.Success();
            }
            
            // JSON'dan Result'a dönüştür
            try
            {
                return JsonSerializer.Deserialize<Result>(content, _jsonOptions);
            }
            catch (JsonException)
            {
                // JSON parse hatası durumunda başarılı say (logout genelde kritik değil)
                return Result.Success();
            }
        }
        catch (Exception ex)
        {
            return Result.Failure($"Çıkış hatası: {ex.Message}");
        }
    }
    
}

