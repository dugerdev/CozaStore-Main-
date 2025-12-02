namespace CozaStore.Application.Common;

/// <summary>
/// Veri dönen API yanıtları için kullanılır.
/// Örnek: Ürün listesi, tek ürün detayı gibi.
/// </summary>
/// <typeparam name="T">Dönecek veri tipi (ProductDto, CategoryDto vb.)</typeparam>

public class Result<T>
{
    
    /// <summary>
    /// API işlemi başarılı mı? true = Başarılı, false = Hata var
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// API'den gelen veri (ürün listesi, kategori vb.)
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Hata durumunda mesaj. Örnek: "Ürün bulunamadı"
    /// </summary>
    public string? ErrorMessage { get; }
    

    
    public Result(bool isSuccess, T? data, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Data = data;
        ErrorMessage = errorMessage;
    }
    

    
    /// <summary>
    /// Başarılı sonuç oluştur. Kullanım: Result<Product>.Success(product)
    /// </summary>
    public static Result<T> Success(T data) => new(true, data, null);
    
    /// <summary>
    /// Hatalı sonuç oluştur. Kullanım: Result<Product>.Failure("Ürün bulunamadı")
    /// </summary>
    public static Result<T> Failure(string error) => new(false, default, error);
    
}


/// <summary>
/// Veri dönmeyen API yanıtları için kullanılır.
/// Örnek: Silme, güncelleme gibi işlemler.
/// </summary>

public class Result
{
    
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    

    
    public Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
    

    
    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    
}

