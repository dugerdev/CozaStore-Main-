using System.Net.Http;
using System.Text;
using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;
using CozaStore.Application.Enums;

namespace CozaStore.Application.Services;

/// <summary>
/// Admin sipariş yönetimi için API ile iletişim kurar.
/// </summary>
public class AdminOrderService
{
    private readonly HttpClient _httpClient;

    public AdminOrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Tüm siparişleri getirir (Admin)
    /// </summary>
    public async Task<Result<List<OrderDto>>?> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<OrderDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(content, options);
            return Result<List<OrderDto>>.Success(orders ?? new List<OrderDto>());
        }
        catch (Exception ex)
        {
            return Result<List<OrderDto>>.Failure($"Siparişler yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Sipariş detayını getirir
    /// </summary>
    public async Task<Result<OrderDto>?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync(id.ToString());
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<OrderDto>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var order = JsonSerializer.Deserialize<OrderDto>(content, options);
            if (order == null)
            {
                return Result<OrderDto>.Failure("Sipariş bulunamadı.");
            }
            
            return Result<OrderDto>.Success(order);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Sipariş yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Sipariş durumunu günceller
    /// </summary>
    public async Task<Result?> UpdateStatusAsync(Guid id, OrderStatus status)
    {
        try
        {
            var json = JsonSerializer.Serialize((int)status);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"{id}/status", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Sipariş durumu güncellenemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Sipariş durumu güncellenemedi: {ex.Message}");
        }
    }
}
