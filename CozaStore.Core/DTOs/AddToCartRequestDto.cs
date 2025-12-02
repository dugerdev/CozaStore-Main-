namespace CozaStore.Core.DTOs;

/// <summary>
/// Sepete ürün ekleme isteği için DTO.
/// </summary>
public class AddToCartRequestDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}


