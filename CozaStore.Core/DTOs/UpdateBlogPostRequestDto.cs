namespace CozaStore.Core.DTOs;

/// <summary>
/// Blog gönderisi güncelleme isteği için DTO.
/// </summary>
public class UpdateBlogPostRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPublished { get; set; }
}

