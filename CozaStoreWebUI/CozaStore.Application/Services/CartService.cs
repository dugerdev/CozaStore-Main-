using System.Text;
using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;


/// <summary>
/// Sepet işlemleri için API ile iletişim kurar.
/// Sepet görüntüleme, ürün ekleme, ürün çıkarma.
/// </summary>

public class CartService
{
    
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = null
    };

    
    public CartService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    

    
    /// <summary>
    /// Kullanıcının sepetini getirir
    /// </summary>
    /// <returns>Sepetteki ürünler</returns>
    
    public async Task<Result<List<CartItemDto>>?> GetMyCartAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var errorResult = JsonSerializer.Deserialize<Result<List<CartItemDto>>>(errorContent, JsonOptions);
                return errorResult ?? Result<List<CartItemDto>>.Failure("Sepet yüklenemedi.");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return Result<List<CartItemDto>>.Failure("Sepet yüklenemedi: Boş yanıt alındı.");
            }
            
            var result = JsonSerializer.Deserialize<Result<List<CartItemDto>>>(content, JsonOptions);
            
            if (result == null)
            {
                return Result<List<CartItemDto>>.Failure("Sepet yüklenemedi: Geçersiz yanıt formatı.");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return Result<List<CartItemDto>>.Failure($"Sepet yüklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Sepete ürün ekler
    /// </summary>
    /// <param name="request">Eklenecek ürün ve miktar</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> AddToCartAsync(AddToCartRequestDto request)
    {
        try
        {
            // DTO'yu JSON'a çevir
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // POST isteği gönder
            var response = await _httpClient.PostAsync("", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // HTTP status code kontrolü
            if (!response.IsSuccessStatusCode)
            {
                // Hata durumunda API'den gelen mesajı parse et
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    try
                    {
                        var errorResult = JsonSerializer.Deserialize<Result>(responseContent, JsonOptions);
                        if (errorResult != null && !string.IsNullOrEmpty(errorResult.ErrorMessage))
                        {
                            return errorResult;
                        }
                    }
                    catch
                    {
                        // JSON parse hatası
                    }
                }
                
                // Genel hata mesajı
                return Result.Failure($"Sepete eklenemedi: HTTP {(int)response.StatusCode} - {response.StatusCode}");
            }
            
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return Result.Failure("Sepete eklenemedi: Boş yanıt alındı.");
            }
            
            var result = JsonSerializer.Deserialize<Result>(responseContent, JsonOptions);
            
            if (result == null)
            {
                return Result.Failure("Sepete eklenemedi: Geçersiz yanıt formatı.");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure($"Sepete eklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Sepetteki ürün miktarını günceller
    /// </summary>
    /// <param name="cartItemId">Sepet öğesi ID'si</param>
    /// <param name="quantity">Yeni miktar</param>
    /// <returns>Başarı/Hata mesajı</returns>
    public async Task<Result?> UpdateQuantityAsync(Guid cartItemId, int quantity)
    {
        try
        {
            var request = new UpdateCartQuantityRequestDto(quantity);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{cartItemId}/quantity", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return Result.Failure("Miktar güncellenemedi: Boş yanıt alındı.");
            }
            
            var result = JsonSerializer.Deserialize<Result>(responseContent, JsonOptions);
            
            if (result == null)
            {
                return Result.Failure("Miktar güncellenemedi: Geçersiz yanıt formatı.");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure($"Miktar güncellenemedi: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Sepetten ürün siler
    /// </summary>
    /// <param name="cartItemId">Sepet öğesi ID'si</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> RemoveFromCartAsync(Guid cartItemId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(cartItemId.ToString());
            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return Result.Failure("Ürün silinemedi: Boş yanıt alındı.");
            }
            
            var result = JsonSerializer.Deserialize<Result>(content, JsonOptions);
            
            if (result == null)
            {
                return Result.Failure("Ürün silinemedi: Geçersiz yanıt formatı.");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ürün silinemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Sepeti temizler
    /// </summary>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> ClearCartAsync()
    {
        try
        {
            var response = await _httpClient.DeleteAsync("clear");
            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return Result.Failure("Sepet temizlenemedi: Boş yanıt alındı.");
            }
            
            var result = JsonSerializer.Deserialize<Result>(content, JsonOptions);
            
            if (result == null)
            {
                return Result.Failure("Sepet temizlenemedi: Geçersiz yanıt formatı.");
            }
            
            return result;
        }
        catch (Exception ex)
        {
            return Result.Failure($"Sepet temizlenemedi: {ex.Message}");
        }
    }
    
}

