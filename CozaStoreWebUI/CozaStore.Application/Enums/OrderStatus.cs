using System.Text.Json.Serialization;

namespace CozaStore.Application.Enums;


/// <summary>
/// Sipariş durumlarını temsil eder.
/// JSON'a string olarak serialize edilir (daha okunabilir).
/// </summary>

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderStatus
{
    /// <summary>
    /// Beklemede - Sipariş verildi, henüz işleme alınmadı
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// İşleniyor - Sipariş hazırlanıyor
    /// </summary>
    Processing = 1,
    
    /// <summary>
    /// Kargoya Verildi - Teslimat yolda
    /// </summary>
    Shipped = 2,
    
    /// <summary>
    /// Teslim Edildi - Sipariş tamamlandı
    /// </summary>
    Delivered = 3,
    
    /// <summary>
    /// İptal Edildi - Sipariş iptal
    /// </summary>
    Cancelled = 4
}

