using System.Text.Json;
using CozaStore.Application.Common;

namespace CozaStore.Application.Services;

/// <summary>
/// Admin dashboard istatistikleri için API ile iletişim kurar.
/// </summary>
public class AdminDashboardService
{
    private readonly HttpClient _httpClient;

    public AdminDashboardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Kullanıcı istatistiklerini getirir (Admin)
    /// </summary>
    public async Task<Result<UserStatisticsDto>?> GetUserStatisticsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("user-statistics");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<UserStatisticsDto>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var statistics = JsonSerializer.Deserialize<UserStatisticsDto>(content, options);
            if (statistics == null)
            {
                return Result<UserStatisticsDto>.Failure("Kullanıcı istatistikleri alınamadı.");
            }
            
            return Result<UserStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<UserStatisticsDto>.Failure($"Kullanıcı istatistikleri yüklenemedi: {ex.Message}");
        }
    }
}

/// <summary>
/// Kullanıcı istatistikleri DTO
/// </summary>
public class UserStatisticsDto
{
    public int RecentRegistrations { get; set; }
    public int TotalUsers { get; set; }
}


