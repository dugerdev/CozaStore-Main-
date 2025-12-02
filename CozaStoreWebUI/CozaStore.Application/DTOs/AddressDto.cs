using CozaStore.Application.Enums;

namespace CozaStore.Application.DTOs;


/// <summary>
/// Kullanıcı adres bilgilerini taşır.
/// </summary>

public record AddressDto(
    Guid Id,
    string UserId,
    string Title,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string District,
    string? PostalCode,
    string Country,
    AddressType? AddressType,
    bool IsDefault
);

