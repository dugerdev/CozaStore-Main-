namespace CozaStore.Core.DTOs;

/// <summary>
/// Blog gönderisi bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class BlogPostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsPublished { get; set; }
    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; }
}
