using System.Text;
using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;

/// <summary>
/// Admin kategori yönetimi için API ile iletişim kurar.
/// Kategori CRUD işlemleri (Create, Read, Update, Delete)
/// </summary>
public class AdminCategoryService
{
    private readonly HttpClient _httpClient;

    public AdminCategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Tüm kategorileri getirir (Admin)
    /// </summary>
    public async Task<Result<List<CategoryDto>>?> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<CategoryDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var categories = JsonSerializer.Deserialize<List<CategoryDto>>(content, options);
            return Result<List<CategoryDto>>.Success(categories ?? new List<CategoryDto>());
        }
        catch (Exception ex)
        {
            return Result<List<CategoryDto>>.Failure($"Kategoriler yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Kategori detayını getirir
    /// </summary>
    public async Task<Result<CategoryDto>?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync(id.ToString());
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<CategoryDto>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var category = JsonSerializer.Deserialize<CategoryDto>(content, options);
            if (category == null)
            {
                return Result<CategoryDto>.Failure("Kategori bulunamadı.");
            }
            
            return Result<CategoryDto>.Success(category);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Kategori yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Yeni kategori oluşturur
    /// </summary>
    public async Task<Result<CategoryDto>?> CreateAsync(CreateCategoryRequestDto request)
    {
        try
        {
            // API Category entity bekliyor
            var categoryData = new
            {
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                IsActive = request.IsActive
            };
            
            var json = JsonSerializer.Serialize(categoryData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // PascalCase
            });
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return Result<CategoryDto>.Failure($"Kategori oluşturulamadı: {responseContent}");
            }
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            // API CategoryDto döndürüyor, direkt deserialize et
            var categoryDto = JsonSerializer.Deserialize<CategoryDto>(responseContent, options);
            
            if (categoryDto == null)
            {
                return Result<CategoryDto>.Failure("Kategori oluşturuldu ancak yanıt parse edilemedi.");
            }
            
            return Result<CategoryDto>.Success(categoryDto);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Kategori oluşturulamadı: {ex.Message}");
        }
    }

    /// <summary>
    /// Kategoriyi günceller
    /// </summary>
    public async Task<Result?> UpdateAsync(UpdateCategoryRequestDto request)
    {
        try
        {
            // API Category entity bekliyor
            var categoryData = new
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                IsActive = request.IsActive
            };
            
            var json = JsonSerializer.Serialize(categoryData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // PascalCase
            });
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(request.Id.ToString(), content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Kategori güncellenemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Kategori güncellenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Kategoriyi siler (soft delete)
    /// </summary>
    public async Task<Result?> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(id.ToString());
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Kategori silinemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Kategori silinemedi: {ex.Message}");
        }
    }
}

