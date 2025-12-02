using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;

/// <summary>
/// Admin mesaj yönetimi için API ile iletişim kurar.
/// </summary>
public class AdminContactService
{
    private readonly HttpClient _httpClient;

    public AdminContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Tüm mesajları getirir (Admin)
    /// </summary>
    public async Task<Result<List<ContactDto>>?> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<ContactDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var contacts = JsonSerializer.Deserialize<List<ContactDto>>(content, options);
            return Result<List<ContactDto>>.Success(contacts ?? new List<ContactDto>());
        }
        catch (Exception ex)
        {
            return Result<List<ContactDto>>.Failure($"Mesajlar yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Okunmamış mesajları getirir (Admin)
    /// </summary>
    public async Task<Result<List<ContactDto>>?> GetUnreadAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("unread");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<ContactDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var contacts = JsonSerializer.Deserialize<List<ContactDto>>(content, options);
            return Result<List<ContactDto>>.Success(contacts ?? new List<ContactDto>());
        }
        catch (Exception ex)
        {
            return Result<List<ContactDto>>.Failure($"Okunmamış mesajlar yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Mesaj detayını getirir
    /// </summary>
    public async Task<Result<ContactDto>?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{id}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<ContactDto>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var contact = JsonSerializer.Deserialize<ContactDto>(content, options);
            if (contact == null)
            {
                return Result<ContactDto>.Failure("Mesaj bulunamadı.");
            }
            
            return Result<ContactDto>.Success(contact);
        }
        catch (Exception ex)
        {
            return Result<ContactDto>.Failure($"Mesaj yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Mesajı okundu olarak işaretler
    /// </summary>
    public async Task<Result?> MarkAsReadAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.PutAsync($"{id}/mark-as-read", null);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Mesaj okundu olarak işaretlenemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Mesaj okundu olarak işaretlenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Mesajı siler
    /// </summary>
    public async Task<Result?> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{id}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Mesaj silinemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Mesaj silinemedi: {ex.Message}");
        }
    }
}


