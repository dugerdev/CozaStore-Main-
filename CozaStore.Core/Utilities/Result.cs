namespace CozaStore.Core.Utilities.Results;

/// <summary>
/// Result Pattern (Sonuç Deseni) - İş katmanından dönen sonuçlar için temel tip.
/// 
/// Bu pattern, iş katmanından (Business Layer) dönen sonuçları standartlaştırır.
/// Her metod, işlemin başarılı/başarısız olduğunu ve mesajını döner.
/// 
/// Avantajları:
/// - Standart yanıt formatı (tutarlılık)
/// - Hata yönetimi kolaylaşır
/// - Controller'da if (result.Success) ile kontrol edilir
/// - Exception handling yerine Result kullanılır (daha kontrollü)
/// 
/// Kullanım:
/// // İş katmanında
/// if (product == null)
///     return new ErrorResult("Ürün bulunamadı.");
/// return new SuccessResult("Ürün eklendi.");
/// 
/// // Controller'da
/// var result = await _productService.AddAsync(product);
/// if (!result.Success)
///     return BadRequest(new { message = result.Message });
/// return Ok();
/// 
/// NOT: Bu abstract class, doğrudan kullanılmaz. 
/// SuccessResult, ErrorResult, DataResult gibi türevleri kullanılır.
/// </summary>
public abstract class Result
{
    /// <summary>
    /// Result sınıfının constructor'ı.
    /// 
    /// Parametreler:
    /// - success: İşlem başarılı mı? (true/false)
    /// - message: Kullanıcıya/üst katmana iletilecek bilgilendirici mesaj
    /// 
    /// Protected çünkü bu abstract class doğrudan instantiate edilmez.
    /// Sadece türev sınıflar (SuccessResult, ErrorResult vb.) kullanılır.
    /// </summary>
    protected Result(bool success, string message)
    {
        Success = success;   // İşlem başarılı mı?
        Message = message;   // Kullanıcıya/üst katmana iletilecek bilgilendirici mesaj
    }

    /// <summary>
    /// İşlem başarılı mı?
    /// 
    /// - true: İşlem başarılı
    /// - false: İşlem başarısız (hata var)
    /// 
    /// Read-only property. Sadece constructor'da set edilir.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Kullanıcıya/üst katmana iletilecek bilgilendirici mesaj.
    /// 
    /// Başarılı işlemlerde: "Ürün eklendi.", "İşlem başarılı." gibi
    /// Başarısız işlemlerde: "Ürün bulunamadı.", "Hata oluştu." gibi
    /// 
    /// Read-only property. Sadece constructor'da set edilir.
    /// </summary>
    public string Message { get; }
}

/// <summary>
/// Başarılı sonuç modeli.
/// 
/// İşlem başarılı olduğunda kullanılır.
/// Success = true olarak ayarlanır.
/// 
/// Kullanım:
/// return new SuccessResult("Ürün başarıyla eklendi.");
/// return new SuccessResult(); // Mesaj opsiyonel
/// 
/// Controller'da:
/// if (result.Success) { ... } // true olur
/// </summary>
public class SuccessResult : Result
{
    /// <summary>
    /// Başarılı sonuç oluşturur.
    /// 
    /// Parametreler:
    /// - message: Başarı mesajı (opsiyonel, boş string olabilir)
    /// 
    /// Success otomatik olarak true yapılır.
    /// </summary>
    public SuccessResult(string message = "") : base(true, message) { }
}

/// <summary>
/// Başarısız sonuç modeli.
/// 
/// İşlem başarısız olduğunda (hata durumunda) kullanılır.
/// Success = false olarak ayarlanır.
/// 
/// Kullanım:
/// return new ErrorResult("Ürün bulunamadı.");
/// return new ErrorResult("Stok yetersiz.");
/// 
/// Controller'da:
/// if (!result.Success) { ... } // false olur
/// </summary>
public class ErrorResult : Result
{
    /// <summary>
    /// Başarısız sonuç oluşturur.
    /// 
    /// Parametreler:
    /// - message: Hata mesajı (opsiyonel, boş string olabilir)
    /// 
    /// Success otomatik olarak false yapılır.
    /// </summary>
    public ErrorResult(string message = "") : base(false, message) { }
}

/// <summary>
/// Veri taşıyan sonuç tipi.
/// 
/// Bu sınıf, Result'a ek olarak Data (veri) bilgisi de taşır.
/// İşlem başarılı/başarısız olabilir, ama her durumda veri dönebilir.
/// 
/// Generic yapı sayesinde herhangi bir tip için kullanılabilir:
/// - DataResult<Product> → Product verisi taşır
/// - DataResult<List<Product>> → Product listesi taşır
/// - DataResult<string> → String verisi taşır
/// 
/// Kullanım:
/// // Başarılı
/// var product = await _repository.GetByIdAsync(id);
/// return new SuccessDataResult<Product>(product, "Ürün bulundu.");
/// 
/// // Başarısız
/// return new ErrorDataResult<Product>(null, "Ürün bulunamadı.");
/// 
/// Controller'da:
/// if (result.Success) {
///     var product = result.Data; // Product nesnesi
/// }
/// 
/// NOT: Bu abstract class, doğrudan kullanılmaz.
/// SuccessDataResult<T> veya ErrorDataResult<T> kullanılır.
/// </summary>
/// <typeparam name="T">Taşınacak veri tipi (Product, List<Product>, string vb.)</typeparam>
public class DataResult<T> : Result
{
    /// <summary>
    /// DataResult sınıfının constructor'ı.
    /// 
    /// Parametreler:
    /// - data: Taşınacak veri (herhangi bir tip olabilir)
    /// - success: İşlem başarılı mı?
    /// - message: Bilgilendirici mesaj (opsiyonel)
    /// 
    /// Protected çünkü bu abstract class doğrudan instantiate edilmez.
    /// </summary>
    public DataResult(T data, bool success, string message = "")
        : base(success, message)
    {
        Data = data;
    }

    /// <summary>
    /// Taşınan veri.
    /// 
    /// Generic tip (T) ne ise, o tip veri taşınır.
    /// 
    /// Örnekler:
    /// - DataResult<Product> → Data = Product nesnesi
    /// - DataResult<List<Product>> → Data = Product listesi
    /// - DataResult<string> → Data = String değeri
    /// 
    /// Read-only property. Sadece constructor'da set edilir.
    /// 
    /// NOT: Hata durumunda null olabilir.
    /// </summary>
    public T Data { get; }
}

/// <summary>
/// Başarılı veri sonuç modeli.
/// 
/// İşlem başarılı olduğunda ve veri döndürmek istediğimizde kullanılır.
/// Success = true olarak ayarlanır.
/// 
/// Kullanım:
/// var product = await _repository.GetByIdAsync(id);
/// return new SuccessDataResult<Product>(product, "Ürün bulundu.");
/// 
/// var products = await _repository.GetAllAsync();
/// return new SuccessDataResult<List<Product>>(products.ToList());
/// 
/// Controller'da:
/// if (result.Success) {
///     var product = result.Data; // Product nesnesi
///     return Ok(product);
/// }
/// </summary>
/// <typeparam name="T">Taşınacak veri tipi</typeparam>
public class SuccessDataResult<T> : DataResult<T>
{
    /// <summary>
    /// Başarılı veri sonucu oluşturur.
    /// 
    /// Parametreler:
    /// - data: Taşınacak veri (T tipinde)
    /// - message: Başarı mesajı (opsiyonel)
    /// 
    /// Success otomatik olarak true yapılır.
    /// </summary>
    public SuccessDataResult(T data, string message = "")
        : base(data, true, message) { }
}

/// <summary>
/// Hatalı veri sonuç modeli.
/// 
/// İşlem başarısız olduğunda kullanılır.
/// Success = false olarak ayarlanır.
/// 
/// Kullanım:
/// return new ErrorDataResult<Product>(null, "Ürün bulunamadı.");
/// 
/// Controller'da:
/// if (!result.Success) {
///     return NotFound(new { message = result.Message });
/// }
/// 
/// NOT: Data genellikle null veya default değer olur.
/// </summary>
/// <typeparam name="T">Taşınacak veri tipi (genellikle null olur)</typeparam>
public class ErrorDataResult<T> : DataResult<T>
{
    /// <summary>
    /// Hatalı veri sonucu oluşturur.
    /// 
    /// Parametreler:
    /// - data: Taşınacak veri (genellikle null veya default değer)
    /// - message: Hata mesajı (opsiyonel)
    /// 
    /// Success otomatik olarak false yapılır.
    /// </summary>
    public ErrorDataResult(T data, string message = "")
        : base(data, false, message) { }
}