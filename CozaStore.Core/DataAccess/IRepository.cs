using System.Linq.Expressions;  // Expression<Func<T, bool>> için
using CozaStore.Entities.Common; // BaseEntity için

namespace CozaStore.Core.DataAccess;

/// <summary>
/// Generic Repository Pattern (Genel Depo Deseni) arayüzü.
/// 
/// Bu interface, tüm entity'ler için ortak CRUD (Create, Read, Update, Delete) 
/// operasyonlarını tanımlar. Generic yapı sayesinde her entity için ayrı 
/// repository interface'i yazmaya gerek kalmaz.
/// 
/// Avantajları:
/// - Kod tekrarını önler (DRY - Don't Repeat Yourself)
/// - Tutarlılık sağlar (tüm repository'ler aynı metodları içerir)
/// - Test edilebilirlik artar (mock repository oluşturulabilir)
/// - Esneklik sağlar (farklı implementasyonlar yapılabilir)
/// 
/// Generic Constraint:
/// - T: BaseEntity'den türemiş olmalı (Id, IsDeleted vb. alanları garanti eder)
/// 
/// Kullanım:
/// IRepository<Product> productRepo = ...;
/// IRepository<Category> categoryRepo = ...;
/// 
/// NOT: Bu sadece bir interface (sözleşme). Gerçek implementasyon 
/// DataAccess katmanında (EfRepositoryBase<T>) yapılır.
/// </summary>
/// <typeparam name="T">
/// BaseEntity'den türeyen herhangi bir entity tipi.
/// Örnek: Product, Category, Order, OrderDetail vb.
/// </typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Primary Key (Id) ile tek bir kayıt getirir.
    /// 
    /// Bu metod, verilen ID'ye sahip kaydı veritabanından getirir.
    /// Soft delete kontrolü yapılır (IsDeleted = false olanlar getirilir).
    /// 
    /// Parametreler:
    /// - id: Aranacak kaydın benzersiz kimliği (GUID)
    /// - cancellationToken: İptal token'ı (async işlemleri iptal etmek için)
    /// 
    /// Dönüş Değeri:
    /// - Kayıt bulunursa: T tipinde entity
    /// - Kayıt bulunamazsa: null
    /// - Soft delete edilmişse: null (otomatik filtrelenir)
    /// 
    /// Kullanım:
    /// var product = await _repository.GetByIdAsync(productId);
    /// if (product != null) { ... }
    /// 
    /// NOT: Bu metod veritabanına SQL sorgusu gönderir.
    /// </summary>
    /// <summary>
    /// Tüm kayıtları getirir (soft delete edilmemiş olanlar).
    /// 
    /// Bu metod, veritabanındaki tüm aktif kayıtları (IsDeleted = false) 
    /// liste olarak döndürür.
    /// 
    /// Dikkat:
    /// - Büyük tablolarda performans sorunu yaratabilir
    /// - Mümkünse FindAsync ile filtreleme yapın
    /// - Pagination (sayfalama) kullanmayı düşünün
    /// 
    /// Dönüş Değeri:
    /// - Tüm aktif kayıtların listesi (IEnumerable<T>)
    /// - Kayıt yoksa: Boş liste
    /// 
    /// Kullanım:
    /// var allProducts = await _repository.GetAllAsync();
    /// foreach (var product in allProducts) { ... }
    /// 
    /// NOT: Bu metod tüm kayıtları memory'ye yükler. Dikkatli kullanın!
    /// </summary>
    /// <summary>
    /// Koşula göre filtrelenmiş kayıtları getirir.
    /// 
    /// Bu metod, verilen koşula (predicate) uyan kayıtları getirir.
    /// Expression<Func<T, bool>> kullanarak LINQ sorguları yazabilirsiniz.
    /// 
    /// Parametreler:
    /// - predicate: Filtreleme koşulu (lambda expression)
    /// - cancellationToken: İptal token'ı
    /// 
    /// Dönüş Değeri:
    /// - Koşula uyan kayıtların listesi (IEnumerable<T>)
    /// - Kayıt yoksa: Boş liste
    /// 
    /// Kullanım:
    /// // Fiyatı 100'den büyük ürünleri getir
    /// var expensiveProducts = await _repository.FindAsync(
    ///     p => p.Price > 100 && p.IsActive
    /// );
    /// 
    /// // Belirli kategoriye ait ürünleri getir
    /// var categoryProducts = await _repository.FindAsync(
    ///     p => p.CategoryId == categoryId
    /// );
    /// 
    /// NOT: Soft delete kontrolü otomatik yapılır (IsDeleted = false).
    /// </summary>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Yeni bir kayıt ekler.
    /// 
    /// Bu metod, verilen entity'yi veritabanına ekler.
    /// Ancak dikkat: Sadece memory'ye ekler, veritabanına yazmaz!
    /// 
    /// ÖNEMLİ:
    /// - AddAsync() çağrıldıktan sonra UnitOfWork.SaveChangesAsync() 
    ///   çağrılmalıdır
    /// - SaveChanges çağrılmadan değişiklikler kalıcı olmaz
    /// 
    /// Parametreler:
    /// - entity: Eklenecek entity nesnesi
    /// - cancellationToken: İptal token'ı
    /// 
    /// Dönüş Değeri:
    /// - Eklenen entity (Id, CreatedDate gibi alanlar otomatik doldurulur)
    /// 
    /// Kullanım:
    /// var product = new Product { Name = "Laptop", Price = 5000 };
    /// await _repository.AddAsync(product);
    /// await _unitOfWork.SaveChangesAsync(); // ŞİMDİ veritabanına yazılır
    /// 
    /// NOT: Entity'nin Id'si yoksa otomatik oluşturulur (BaseEntity'de).
    /// </summary>
    /// <summary>
    /// Birden fazla kaydı toplu olarak ekler.
    /// 
    /// Bu metod, verilen entity listesini veritabanına ekler.
    /// Ancak dikkat: Sadece memory'ye ekler, veritabanına yazmaz!
    /// 
    /// ÖNEMLİ:
    /// - AddRangeAsync() çağrıldıktan sonra UnitOfWork.SaveChangesAsync() 
    ///   çağrılmalıdır
    /// - SaveChanges çağrılmadan değişiklikler kalıcı olmaz
    /// 
    /// Parametreler:
    /// - entities: Eklenecek entity listesi
    /// - cancellationToken: İptal token'ı
    /// 
    /// Kullanım:
    /// var orderDetails = new List<OrderDetail> { detail1, detail2, detail3 };
    /// await _repository.AddRangeAsync(orderDetails);
    /// await _unitOfWork.SaveChangesAsync(); // ŞİMDİ veritabanına yazılır
    /// 
    /// Performans: Tek tek AddAsync çağırmaktan daha hızlıdır.
    /// </summary>
    /// <summary>
    /// Mevcut bir kaydı günceller.
    /// 
    /// Bu metod, verilen entity'yi veritabanında günceller.
    /// Entity'nin Id'si mevcut bir kayda ait olmalıdır.
    /// 
    /// ÖNEMLİ:
    /// - UpdateAsync() çağrıldıktan sonra UnitOfWork.SaveChangesAsync() 
    ///   çağrılmalıdır
    /// - SaveChanges çağrılmadan değişiklikler kalıcı olmaz
    /// 
    /// Parametreler:
    /// - entity: Güncellenecek entity nesnesi (Id alanı dolu olmalı)
    /// - cancellationToken: İptal token'ı
    /// 
    /// Kullanım:
    /// var product = await _repository.GetByIdAsync(productId);
    /// product.Price = 6000;
    /// product.UpdatedDate = DateTime.UtcNow;
    /// await _repository.UpdateAsync(product);
    /// await _unitOfWork.SaveChangesAsync();
    /// 
    /// NOT: EF Core ChangeTracker entity'yi "Modified" olarak işaretler.
    /// </summary>
    /// <summary>
    /// Bir kaydı fiziksel olarak siler (Hard Delete).
    /// 
    /// DİKKAT: Bu metod kaydı veritabanından tamamen siler!
    /// Geri getirme imkanı yoktur. Mümkünse SoftDeleteAsync kullanın.
    /// 
    /// ÖNEMLİ:
    /// - DeleteAsync() çağrıldıktan sonra UnitOfWork.SaveChangesAsync() 
    ///   çağrılmalıdır
    /// - SaveChanges çağrılmadan silme işlemi kalıcı olmaz
    /// 
    /// Parametreler:
    /// - entity: Silinecek entity nesnesi
    /// - cancellationToken: İptal token'ı
    /// 
    /// Kullanım:
    /// var product = await _repository.GetByIdAsync(productId);
    /// await _repository.DeleteAsync(product);
    /// await _unitOfWork.SaveChangesAsync();
    /// 
    /// NOT: İlişkili kayıtlar varsa hata alabilirsiniz (Foreign Key constraint).
    /// </summary>
    /// <summary>
    /// Birden fazla kaydı toplu olarak siler (Hard Delete).
    /// 
    /// DİKKAT: Bu metod kayıtları veritabanından tamamen siler!
    /// Geri getirme imkanı yoktur. Mümkünse SoftDeleteAsync kullanın.
    /// 
    /// ÖNEMLİ:
    /// - DeleteRangeAsync() çağrıldıktan sonra UnitOfWork.SaveChangesAsync() 
    ///   çağrılmalıdır
    /// - SaveChanges çağrılmadan silme işlemi kalıcı olmaz
    /// 
    /// Parametreler:
    /// - entities: Silinecek entity listesi
    /// - cancellationToken: İptal token'ı
    /// 
    /// Kullanım:
    /// var itemsToDelete = await _repository.FindAsync(x => x.IsActive == false);
    /// await _repository.DeleteRangeAsync(itemsToDelete);
    /// await _unitOfWork.SaveChangesAsync();
    /// 
    /// Performans: Tek tek DeleteAsync çağırmaktan daha hızlıdır.
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bir kaydı mantıksal olarak siler (Soft Delete).
    /// 
    /// Bu metod, kaydı fiziksel olarak silmez, sadece:
    /// - IsDeleted = true
    /// - IsActive = false
    /// - DeletedDate = DateTime.UtcNow
    /// alanlarını günceller.
    /// 
    /// Avantajları:
    /// - Veri kaybı olmaz (geri getirme imkanı)
    /// - Audit (denetim) için önemli
    /// - İlişkili kayıtların bütünlüğü korunur
    /// 
    /// ÖNEMLİ:
    /// - SoftDeleteAsync() çağrıldıktan sonra UnitOfWork.SaveChangesAsync() 
    ///   çağrılmalıdır
    /// - SaveChanges çağrılmadan silme işlemi kalıcı olmaz
    /// 
    /// Parametreler:
    /// - id: Silinecek kaydın benzersiz kimliği (GUID)
    /// - cancellationToken: İptal token'ı
    /// 
    /// Dönüş Değeri:
    /// - Kayıt bulunamazsa: Hiçbir şey yapılmaz (sessizce başarısız olur)
    /// - Kayıt bulunursa: Soft delete uygulanır
    /// 
    /// Kullanım:
    /// await _repository.SoftDeleteAsync(productId);
    /// await _unitOfWork.SaveChangesAsync();
    /// 
    /// NOT: Soft delete edilen kayıtlar GetByIdAsync, GetAllAsync, FindAsync 
    /// metodlarında otomatik olarak filtrelenir.
    /// </summary>
    Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Belirtilen ID'ye sahip kayıt var mı kontrol eder.
    /// 
    /// Bu metod, verilen ID'ye sahip aktif (IsDeleted = false) bir kayıt 
    /// olup olmadığını kontrol eder.
    /// 
    /// Parametreler:
    /// - id: Kontrol edilecek kaydın benzersiz kimliği (GUID)
    /// - cancellationToken: İptal token'ı
    /// 
    /// Dönüş Değeri:
    /// - true: Kayıt var ve aktif
    /// - false: Kayıt yok veya soft delete edilmiş
    /// 
    /// Kullanım:
    /// if (await _repository.ExistsAsync(productId))
    /// {
    ///     // Kayıt var, işlem yapabilirsiniz
    /// }
    /// 
    /// Performans:
    /// - GetByIdAsync'den daha hızlıdır (sadece varlık kontrolü yapar)
    /// - Count() yerine Any() kullanır (daha verimli)
    /// 
    /// NOT: Soft delete edilmiş kayıtlar "yok" kabul edilir.
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
