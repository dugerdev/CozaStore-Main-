using CozaStore.Entities.Enums;

namespace CozaStore.Core.DTOs;

/// <summary>
/// Adres bilgilerini API'den döndürmek için kullanılan DTO.
/// </summary>
public class AddressDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string Country { get; set; } = string.Empty;
    public AddressType? AddressType { get; set; }
    public bool IsDefault { get; set; }
}


