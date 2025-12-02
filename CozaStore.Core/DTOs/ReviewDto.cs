namespace CozaStore.Core.DTOs;

/// <summary>
/// Yorum/Değerlendirme bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedDate { get; set; }
}


