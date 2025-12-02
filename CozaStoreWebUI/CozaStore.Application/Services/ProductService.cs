using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;


/// <summary>
/// Ürün işlemleri için API ile iletişim kurar.
/// Ürün listeleme, detay görüntüleme, kategori filtreleme.
/// </summary>

public class ProductService
{
    
    private readonly HttpClient _httpClient;
    

    
    public ProductService(HttpClient httpClient)
    {
        // BaseAddress Program.cs'de zaten set ediliyor, burada tekrar set etmeye gerek yok
        // Sadece HttpClient'ı sakla
        _httpClient = httpClient;
    }
    

    
    /// <summary>
    /// Tüm ürünleri getirir
    /// </summary>
    /// <returns>Ürün listesi</returns>
    
    public async Task<Result<List<ProductDto>>?> GetAllAsync()
    {
        try
        {
            // API'ye GET isteği gönder
            var response = await _httpClient.GetAsync("");
            
            // HTTP status code kontrolü
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<ProductDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            // Yanıtı string olarak oku
            var content = await response.Content.ReadAsStringAsync();
            
            // JSON deserialize options (PropertyNamingPolicy null - PascalCase korunur)
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
            
            // API direkt List<ProductDto> döndürüyor, Result sarmalı değil
            var products = JsonSerializer.Deserialize<List<ProductDto>>(content, options);
            
            if (products == null)
            {
                return Result<List<ProductDto>>.Failure("API'den veri alınamadı");
            }
            
            // Result'a sar ve döndür
            return Result<List<ProductDto>>.Success(products);
        }
        catch (Exception ex)
        {
            // Hata durumunda null dön
            return Result<List<ProductDto>>.Failure($"API Hatası: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// ID'ye göre tek ürün getirir
    /// </summary>
    /// <param name="id">Ürün ID'si</param>
    /// <returns>Ürün detayı</returns>
    
    public async Task<Result<ProductDto>?> GetByIdAsync(Guid id)
    {
        try
        {
            // URL: api/products/{id}
            var response = await _httpClient.GetAsync(id.ToString());
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<ProductDto>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
            
            // API direkt ProductDto döndürüyor
            var product = JsonSerializer.Deserialize<ProductDto>(content, options);
            
            if (product == null)
            {
                return Result<ProductDto>.Failure("Ürün bulunamadı");
            }
            
            return Result<ProductDto>.Success(product);
        }
        catch (Exception ex)
        {
            return Result<ProductDto>.Failure($"Ürün bulunamadı: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Kategoriye göre ürünleri getirir
    /// </summary>
    /// <param name="categoryId">Kategori ID'si</param>
    /// <returns>Kategorideki ürünler</returns>
    
    public async Task<Result<List<ProductDto>>?> GetByCategoryAsync(Guid categoryId)
    {
        try
        {
            // URL: api/products/category/{categoryId}
            var response = await _httpClient.GetAsync($"category/{categoryId}");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return Result<List<ProductDto>>.Failure($"API Hatası: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
            
            // API direkt List<ProductDto> döndürüyor
            var products = JsonSerializer.Deserialize<List<ProductDto>>(content, options);
            
            if (products == null)
            {
                return Result<List<ProductDto>>.Failure("Kategori ürünleri alınamadı");
            }
            
            return Result<List<ProductDto>>.Success(products);
        }
        catch (Exception ex)
        {
            return Result<List<ProductDto>>.Failure($"Kategori ürünleri bulunamadı: {ex.Message}");
        }
    }
    
}

