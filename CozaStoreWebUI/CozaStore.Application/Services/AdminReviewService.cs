using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;

/// <summary>
/// Admin yorum yönetimi için API ile iletişim kurar.
/// </summary>
public class AdminReviewService
{
    private readonly HttpClient _httpClient;

    public AdminReviewService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Tüm yorumları getirir (Admin)
    /// </summary>
    public async Task<Result<List<ReviewDto>>?> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<ReviewDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var reviews = JsonSerializer.Deserialize<List<ReviewDto>>(content, options);
            return Result<List<ReviewDto>>.Success(reviews ?? new List<ReviewDto>());
        }
        catch (Exception ex)
        {
            return Result<List<ReviewDto>>.Failure($"Yorumlar yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Yorumu onaylar
    /// </summary>
    public async Task<Result?> ApproveAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"{id}/approve", null);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Yorum onaylanamadı: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Yorum onaylanamadı: {ex.Message}");
        }
    }

    /// <summary>
    /// Yorumu reddeder
    /// </summary>
    public async Task<Result?> RejectAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"{id}/reject", null);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Yorum reddedilemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Yorum reddedilemedi: {ex.Message}");
        }
    }
}

