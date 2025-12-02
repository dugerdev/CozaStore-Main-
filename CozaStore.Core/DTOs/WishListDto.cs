namespace CozaStore.Core.DTOs;

/// <summary>
/// İstek listesi öğesi bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class WishListDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public ProductDto? Product { get; set; }
    public DateTime CreatedDate { get; set; }
}


