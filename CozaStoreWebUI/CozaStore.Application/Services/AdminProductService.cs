using System.Text;
using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;

/// <summary>
/// Admin ürün yönetimi için API ile iletişim kurar.
/// Ürün CRUD işlemleri (Create, Read, Update, Delete)
/// </summary>
public class AdminProductService
{
    private readonly HttpClient _httpClient;

    public AdminProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Tüm ürünleri getirir (Admin)
    /// </summary>
    public async Task<Result<List<ProductDto>>?> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<ProductDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var products = JsonSerializer.Deserialize<List<ProductDto>>(content, options);
            return Result<List<ProductDto>>.Success(products ?? new List<ProductDto>());
        }
        catch (Exception ex)
        {
            return Result<List<ProductDto>>.Failure($"Ürünler yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Ürün detayını getirir
    /// </summary>
    public async Task<Result<ProductDto>?> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync(id.ToString());
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<ProductDto>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var product = JsonSerializer.Deserialize<ProductDto>(content, options);
            if (product == null)
            {
                return Result<ProductDto>.Failure("Ürün bulunamadı.");
            }
            
            return Result<ProductDto>.Success(product);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"Ürün yüklenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Yeni ürün oluşturur
    /// </summary>
    public async Task<Result<ProductDto>?> CreateAsync(CreateProductRequestDto request)
    {
        try
        {
            // API Product entity bekliyor, sadece gerekli alanları gönder
            // Navigation property'leri göndermiyoruz (CategoryId yeterli)
            var productData = new
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive
            };
            
            var json = JsonSerializer.Serialize(productData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // PascalCase
            });
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return Result<ProductDto>.Failure($"Ürün oluşturulamadı: {responseContent}");
            }
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            // API ProductDto döndürüyor, direkt deserialize et
            var productDto = JsonSerializer.Deserialize<ProductDto>(responseContent, options);
            
            if (productDto == null)
            {
                return Result<ProductDto>.Failure("Ürün oluşturuldu ancak yanıt parse edilemedi.");
            }
            
            return Result<ProductDto>.Success(productDto);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"Ürün oluşturulamadı: {ex.Message}");
        }
    }

    /// <summary>
    /// Ürünü günceller
    /// </summary>
    public async Task<Result?> UpdateAsync(UpdateProductRequestDto request)
    {
        try
        {
            // API Product entity bekliyor, sadece gerekli alanları gönder
            // Navigation property'leri göndermiyoruz (CategoryId yeterli)
            var productData = new
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive
            };
            
            var json = JsonSerializer.Serialize(productData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // PascalCase
            });
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(request.Id.ToString(), content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Ürün güncellenemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ürün güncellenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Ürünü siler (soft delete)
    /// </summary>
    public async Task<Result?> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(id.ToString());
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result.Failure($"Ürün silinemedi: {errorContent}");
            }
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Ürün silinemedi: {ex.Message}");
        }
    }
}
