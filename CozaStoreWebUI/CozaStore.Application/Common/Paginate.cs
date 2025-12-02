namespace CozaStore.Application.Common;

/// <summary>
/// API'den gelen sayfalı verileri tutmak için kullanılır.
/// Örnek: 100 ürün var, sayfa başına 12 ürün göster
/// </summary>
/// <typeparam name="T">Listelenecek veri tipi (ProductDto, OrderDto vb.)</typeparam>

public class Paginate<T> where T : class
{
    
    /// <summary>
    /// Sayfa başına kaç öğe? (Örnek: 12 ürün)
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Hangi sayfadayız? (0: İlk sayfa, 1: İkinci sayfa)
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Toplam kaç öğe var? (Örnek: 100 ürün)
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Toplam kaç sayfa var? (Count / Size)
    /// </summary>
    public int Pages { get; set; }
    

    
    /// <summary>
    /// Bu sayfadaki öğeler
    /// </summary>
    public ICollection<T> Items { get; set; } = [];
    

    
    /// <summary>
    /// Önceki sayfa var mı? Örnek: 2. sayfadaysak true (1. sayfaya gidebiliriz)
    /// </summary>
    public bool HasPrevious => Index > 0;

    /// <summary>
    /// Sonraki sayfa var mı? Örnek: Son sayfa değilsek true
    /// </summary>
    public bool HasNext => Index + 1 < Pages;
    
}

