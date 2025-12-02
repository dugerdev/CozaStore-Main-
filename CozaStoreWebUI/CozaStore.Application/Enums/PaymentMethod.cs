using System.Text.Json.Serialization;

namespace CozaStore.Application.Enums;


/// <summary>
/// Ödeme yöntemlerini temsil eder.
/// </summary>

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentMethod
{
    /// <summary>
    /// Kredi Kartı
    /// </summary>
    CreditCard = 0,
    
    /// <summary>
    /// Banka Transferi (EFT/Havale)
    /// </summary>
    BankTransfer = 1,
    
    /// <summary>
    /// Kapıda Ödeme (Nakit/Kart)
    /// </summary>
    CashOnDelivery = 2,
    
    /// <summary>
    /// Dijital Cüzdan (PayPal, PayTR vb.)
    /// </summary>
    DigitalWallet = 3
}

