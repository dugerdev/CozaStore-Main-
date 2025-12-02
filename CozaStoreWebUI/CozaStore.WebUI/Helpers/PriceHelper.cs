namespace CozaStore.WebUI.Helpers;

/// <summary>
/// Fiyat formatlama için helper sınıfı.
/// </summary>
public static class PriceHelper
{
    /// <summary>
    /// Fiyatı dolar formatında gösterir ($XX.XX)
    /// </summary>
    public static string FormatAsDollar(decimal price)
    {
        return $"${price:F2}";
    }

    /// <summary>
    /// Fiyatı TL formatında gösterir (₺XX.XX)
    /// </summary>
    public static string FormatAsTurkishLira(decimal price)
    {
        return $"₺{price:F2}";
    }
}

