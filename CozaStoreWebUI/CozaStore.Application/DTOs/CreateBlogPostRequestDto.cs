namespace CozaStore.Application.DTOs;

/// <summary>
/// Yeni blog gönderisi oluşturma isteği için DTO.
/// </summary>
public record CreateBlogPostRequestDto(
    string Title,
    string Content,
    string? ImageUrl,
    bool IsPublished
);


