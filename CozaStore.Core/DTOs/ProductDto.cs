namespace CozaStore.Core.DTOs;

/// <summary>
/// Ürün bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? ImageUrl { get; set; }
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}


