using CozaStore.Application.Common;
using CozaStore.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace CozaStore.Application.Services;

/// <summary>
/// Public blog gönderileri için API ile iletişim kurar.
/// </summary>
public class BlogService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BlogService> _logger;

    public BlogService(HttpClient httpClient, ILogger<BlogService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <summary>
    /// Yayınlanmış tüm blog gönderilerini getirir.
    /// </summary>
    public async Task<Result<List<BlogPostDto>>> GetPublishedAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("published");
            
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
            var response = await _httpClient.GetAsync(id.ToString());
            
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
}
