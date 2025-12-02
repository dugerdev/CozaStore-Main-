using System.Text;
using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;


/// <summary>
/// Ürün yorum işlemleri için API ile iletişim kurar.
/// Kullanıcılar ürünlere yorum yapabilir, puan verebilir.
/// </summary>

public class ReviewService
{
    
    private readonly HttpClient _httpClient;
    

    
    public ReviewService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    

    
    /// <summary>
    /// Ürünün yorumlarını getirir
    /// </summary>
    /// <param name="productId">Ürün ID'si</param>
    /// <returns>Ürüne ait yorum listesi</returns>
    
    public async Task<Result<List<ReviewDto>>?> GetProductReviewsAsync(Guid productId)
    {
        try
        {
            // GET api/reviews/product/{productId}
            var response = await _httpClient.GetAsync($"product/{productId}");
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result<List<ReviewDto>>>(content);
        }
        catch (Exception ex)
        {
            return Result<List<ReviewDto>>.Failure($"Yorumlar yüklenemedi: {ex.Message}");
        }
    }

    
    /// <summary>
    /// Yeni yorum ekler
    /// </summary>
    /// <param name="review">Yorum bilgileri (ProductId, Rating, Comment)</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> CreateReviewAsync(ReviewDto review)
    {
        try
        {
            var json = JsonSerializer.Serialize(review);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // POST api/reviews
            var response = await _httpClient.PostAsync("", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result>(responseContent);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Yorum eklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Yorumu siler
    /// </summary>
    /// <param name="reviewId">Yorum ID'si</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> DeleteReviewAsync(Guid reviewId)
    {
        try
        {
            // DELETE api/reviews/{reviewId}
            var response = await _httpClient.DeleteAsync(reviewId.ToString());
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result>(content);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Yorum silinemedi: {ex.Message}");
        }
    }
    
}

