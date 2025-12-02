namespace CozaStore.Core.DTOs;

/// <summary>
/// Yeni yorum oluşturma isteği için DTO.
/// </summary>
public class CreateReviewRequestDto
{
    public Guid ProductId { get; set; }
    public string? Title { get; set; }
    public string? Comment { get; set; }
    public int Rating { get; set; }
}


