using System.Text.Json.Serialization;

namespace CozaStore.Application.Enums;


/// <summary>
/// Adres tiplerini temsil eder.
/// </summary>

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AddressType
{
    /// <summary>
    /// Ev Adresi
    /// </summary>
    Home = 0,
    
    /// <summary>
    /// İş Adresi
    /// </summary>
    Work = 1,
    
    /// <summary>
    /// Diğer (Özel adres)
    /// </summary>
    Other = 2
}

