using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;


/// <summary>
/// İstek listesi işlemleri için API ile iletişim kurar.
/// Kullanıcılar beğendikleri ürünleri favorilere ekleyebilir.
/// </summary>

public class WishListService
{
    
    private readonly HttpClient _httpClient;
    

    
    public WishListService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    

    
    /// <summary>
    /// Kullanıcının istek listesini getirir
    /// </summary>
    /// <returns>İstek listesindeki ürünler</returns>
    
    public async Task<Result<List<WishListDto>>?> GetMyWishListAsync()
    {
        try
        {
            // GET api/wishlist
            var response = await _httpClient.GetAsync("");
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result<List<WishListDto>>>(content);
        }
        catch (Exception ex)
        {
            return Result<List<WishListDto>>.Failure($"İstek listesi yüklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// İstek listesine ürün ekler
    /// </summary>
    /// <param name="productId">Eklenecek ürünün ID'si</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> AddToWishListAsync(Guid productId)
    {
        try
        {
            // POST api/wishlist/{productId}
            var response = await _httpClient.PostAsync(productId.ToString(), null);
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result>(content);
        }
        catch (Exception ex)
        {
            return Result.Failure($"İstek listesine eklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// İstek listesinden ürün çıkarır
    /// </summary>
    /// <param name="wishListItemId">Çıkarılacak öğenin ID'si</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> RemoveFromWishListAsync(Guid wishListItemId)
    {
        try
        {
            // DELETE api/wishlist/{wishListItemId}
            var response = await _httpClient.DeleteAsync(wishListItemId.ToString());
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result>(content);
        }
        catch (Exception ex)
        {
            return Result.Failure($"İstek listesinden çıkarılamadı: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Ürünün istek listesinde olup olmadığını kontrol eder
    /// </summary>
    /// <param name="productId">Kontrol edilecek ürün ID'si</param>
    /// <returns>true/false</returns>
    
    public async Task<Result<bool>?> IsInWishListAsync(Guid productId)
    {
        try
        {
            // GET api/wishlist/check/{productId}
            var response = await _httpClient.GetAsync($"check/{productId}");
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result<bool>>(content);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Kontrol edilemedi: {ex.Message}");
        }
    }
    
}

