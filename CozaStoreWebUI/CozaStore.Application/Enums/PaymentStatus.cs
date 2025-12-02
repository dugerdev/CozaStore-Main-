using System.Text.Json.Serialization;

namespace CozaStore.Application.Enums;


/// <summary>
/// Ödeme durumlarını temsil eder.
/// </summary>

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    /// <summary>
    /// Ödenmedi - Ödeme bekleniyor
    /// </summary>
    Unpaid = 0,
    
    /// <summary>
    /// Ödendi - Ödeme tamamlandı
    /// </summary>
    Paid = 1,
    
    /// <summary>
    /// İade Edildi - Tam iade
    /// </summary>
    Refunded = 2,
    
    /// <summary>
    /// Kısmen İade Edildi - Partial refund
    /// </summary>
    PartiallyRefunded = 3
}

