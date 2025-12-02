using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;


/// <summary>
/// Kategori işlemleri için API ile iletişim kurar.
/// </summary>

public class CategoryService
{
    
    private readonly HttpClient _httpClient;
    

    
    public CategoryService(HttpClient httpClient)
    {
        // BaseAddress Program.cs'de zaten set ediliyor, burada tekrar set etmeye gerek yok
        // Sadece HttpClient'ı sakla
        _httpClient = httpClient;
    }
    

    
    /// <summary>
    /// Tüm kategorileri getirir
    /// </summary>
    /// <returns>Kategori listesi</returns>
    
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
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
            
            // API direkt List<CategoryDto> döndürüyor, Result sarmalı değil
            var categories = JsonSerializer.Deserialize<List<CategoryDto>>(content, options);
            
            if (categories == null)
            {
                return Result<List<CategoryDto>>.Failure("Kategoriler alınamadı");
            }
            
            return Result<List<CategoryDto>>.Success(categories);
        }
        catch (Exception ex)
        {
            return Result<List<CategoryDto>>.Failure($"Kategoriler yüklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// ID'ye göre tek kategori getirir
    /// </summary>
    /// <param name="id">Kategori ID'si</param>
    /// <returns>Kategori detayı</returns>
    
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
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
            
            // API direkt CategoryDto döndürüyor
            var category = JsonSerializer.Deserialize<CategoryDto>(content, options);
            
            if (category == null)
            {
                return Result<CategoryDto>.Failure("Kategori bulunamadı");
            }
            
            return Result<CategoryDto>.Success(category);
        }
        catch (Exception ex)
        {
            return Result<CategoryDto>.Failure($"Kategori bulunamadı: {ex.Message}");
        }
    }
    
}

