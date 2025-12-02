using CozaStore.Application.Enums;

namespace CozaStore.Application.DTOs;


/// <summary>
/// Sipariş bilgilerini taşır.
/// </summary>

public record OrderDto(
    Guid Id,
    string OrderNumber,
    DateTime OrderDate,
    decimal TotalAmount,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    string UserId
);

