namespace CozaStore.Core.DTOs;

/// <summary>
/// Sipariş detay bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class OrderDetailDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
}


