namespace CozaStore.Application.DTOs;

/// <summary>
/// Blog gönderisi bilgilerini taşır.
/// API'den gelen format ile uyumlu olmalı.
/// </summary>
public record BlogPostDto(
    Guid Id,
    string Title,
    string Content,
    string? ImageUrl,
    bool IsPublished,
    Guid AuthorId,
    string? AuthorName,
    DateTime CreatedDate,
    DateTime? UpdatedDate,
    bool IsActive
);

