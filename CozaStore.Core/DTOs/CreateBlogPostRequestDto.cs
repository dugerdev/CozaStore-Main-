namespace CozaStore.Core.DTOs;

/// <summary>
/// Yeni blog gönderisi oluşturma isteği için DTO.
/// </summary>
public class CreateBlogPostRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPublished { get; set; }
}

