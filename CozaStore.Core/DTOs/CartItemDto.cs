namespace CozaStore.Core.DTOs;

/// <summary>
/// Sepet öğesi bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class CartItemDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public decimal? ProductPrice { get; set; }
    public string? ProductImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal? SubTotal { get; set; }
}


