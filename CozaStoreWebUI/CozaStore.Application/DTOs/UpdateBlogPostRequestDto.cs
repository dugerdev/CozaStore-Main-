namespace CozaStore.Application.DTOs;

/// <summary>
/// Blog gönderisi güncelleme isteği için DTO.
/// </summary>
public record UpdateBlogPostRequestDto(
    Guid Id,
    string Title,
    string Content,
    string? ImageUrl,
    bool IsPublished
);


