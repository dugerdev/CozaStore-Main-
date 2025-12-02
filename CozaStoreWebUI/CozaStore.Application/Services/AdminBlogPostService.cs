using CozaStore.Application.Common;
using CozaStore.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CozaStore.Application.Services;

/// <summary>
/// Admin panel için blog gönderileri API işlemlerini yöneten servis.
/// </summary>
public class AdminBlogPostService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AdminBlogPostService> _logger;

    public AdminBlogPostService(
        HttpClient httpClient,
        ILogger<AdminBlogPostService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Tüm blog gönderilerini getirir.
    /// </summary>
    public async Task<Result<List<BlogPostDto>>> GetAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            
            if (response.IsSuccessStatusCode)
            {
                var blogPosts = await response.Content.ReadFromJsonAsync<List<BlogPostDto>>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return Result<List<BlogPostDto>>.Success(blogPosts ?? new List<BlogPostDto>());
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Blog gönderileri getirilemedi. Status: {Status}, Content: {Content}", 
                response.StatusCode, errorContent);
            
            return Result<List<BlogPostDto>>.Failure("Blog gönderileri getirilemedi.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blog gönderileri getirilirken hata oluştu.");
            return Result<List<BlogPostDto>>.Failure("Blog gönderileri getirilirken hata oluştu.");
        }
    }

    /// <summary>
    /// ID'ye göre blog gönderisini getirir.
    /// </summary>
    public async Task<Result<BlogPostDto>> GetByIdAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var blogPost = await response.Content.ReadFromJsonAsync<BlogPostDto>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (blogPost != null)
                {
                    return Result<BlogPostDto>.Success(blogPost);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Blog gönderisi getirilemedi. Status: {Status}, Content: {Content}", 
                response.StatusCode, errorContent);
            
            return Result<BlogPostDto>.Failure("Blog gönderisi bulunamadı.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blog gönderisi getirilirken hata oluştu.");
            return Result<BlogPostDto>.Failure("Blog gönderisi getirilirken hata oluştu.");
        }
    }

    /// <summary>
    /// Yeni blog gönderisi oluşturur.
    /// </summary>
    public async Task<Result<BlogPostDto>> CreateAsync(CreateBlogPostRequestDto request)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // API PascalCase bekliyor
            });
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("", content);
            
            if (response.IsSuccessStatusCode)
            {
                var blogPost = await response.Content.ReadFromJsonAsync<BlogPostDto>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (blogPost != null)
                {
                    return Result<BlogPostDto>.Success(blogPost);
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Blog gönderisi oluşturulamadı. Status: {Status}, Content: {Content}", 
                response.StatusCode, errorContent);
            
            return Result<BlogPostDto>.Failure($"Blog gönderisi oluşturulamadı: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blog gönderisi oluşturulurken hata oluştu.");
            return Result<BlogPostDto>.Failure("Blog gönderisi oluşturulurken hata oluştu.");
        }
    }

    /// <summary>
    /// Blog gönderisini günceller.
    /// </summary>
    public async Task<Result> UpdateAsync(Guid id, UpdateBlogPostRequestDto request)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null // API PascalCase bekliyor
            });
            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{id}", content);
            
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Blog gönderisi güncellenemedi. Status: {Status}, Content: {Content}", 
                response.StatusCode, errorContent);
            
            return Result.Failure($"Blog gönderisi güncellenemedi: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blog gönderisi güncellenirken hata oluştu.");
            return Result.Failure("Blog gönderisi güncellenirken hata oluştu.");
        }
    }

    /// <summary>
    /// Blog gönderisini siler.
    /// </summary>
    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{id}");
            
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Blog gönderisi silinemedi. Status: {Status}, Content: {Content}", 
                response.StatusCode, errorContent);
            
            return Result.Failure($"Blog gönderisi silinemedi: {errorContent}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blog gönderisi silinirken hata oluştu.");
            return Result.Failure("Blog gönderisi silinirken hata oluştu.");
        }
    }
}
