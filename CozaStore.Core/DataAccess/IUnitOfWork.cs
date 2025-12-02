using CozaStore.Entities.Entities; // Entity tipleri için

namespace CozaStore.Core.DataAccess
{
    /// <summary>
    /// Unit of Work Pattern (İş Birimi Deseni) arayüzü.
    /// 
    /// Bu pattern, birden fazla repository'yi tek bir yerden yönetmeyi sağlar.
    /// Ayrıca transaction yönetimi ve SaveChanges işlemlerini merkezileştirir.
    /// 
    /// Avantajları:
    /// - Tüm repository'lere tek noktadan erişim
    /// - Transaction yönetimi (ACID özellikleri)
    /// - SaveChanges tek seferde çağrılır (performans)
    /// - Tutarlılık sağlar (tüm işlemler birlikte başarılı/başarısız olur)
    /// 
    /// Kullanım Senaryosu:
    /// // Sipariş oluştururken hem Order hem OrderDetail kaydetmemiz gerekiyor
    /// await _unitOfWork.Orders.AddAsync(order);
    /// await _unitOfWork.OrderDetails.AddRangeAsync(orderDetails);
    /// await _unitOfWork.SaveChangesAsync(); // İKİSİ BİRLİKTE kaydedilir
    /// 
    /// Transaction Kullanımı:
    /// await _unitOfWork.BeginTransactionAsync();
    /// try {
    ///     // Birden fazla işlem
    ///     await _unitOfWork.SaveChangesAsync();
    ///     await _unitOfWork.CommitTransactionAsync();
    /// } catch {
    ///     await _unitOfWork.RollbackTransactionAsync();
    /// }
    /// 
    /// NOT: IAsyncDisposable implement eder, bu sayede using bloğu ile 
    /// otomatik dispose edilebilir.
    /// </summary>
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Product entity'si için repository erişimi.
        /// 
        /// Her property, ilk çağrıldığında ilgili repository'yi oluşturur
        /// ve cache'ler. Aynı repository tekrar tekrar oluşturulmaz.
        /// 
        /// Kullanım:
        /// var product = await _unitOfWork.Products.GetByIdAsync(id);
        /// await _unitOfWork.Products.AddAsync(newProduct);
        /// </summary>
        /// <summary>
        /// Category entity'si için repository erişimi.
        /// </summary>
        /// <summary>
        /// Order entity'si için repository erişimi.
        /// </summary>
        /// <summary>
        /// OrderDetail entity'si için repository erişimi.
        /// </summary>
        /// <summary>
        /// CartItem entity'si için repository erişimi.
        /// </summary>
        /// <summary>
        /// Address entity'si için repository erişimi.
        /// </summary>
        /// <summary>
        /// Review entity'si için repository erişimi.
        /// </summary>
        /// <summary>
        /// WishList entity'si için repository erişimi.
        /// </summary>
        /// <summary>
        /// Contact entity'si için repository erişimi.
        /// </summary>
        IRepository<Product> Products { get; }
        IRepository<Category> Categories { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }
        IRepository<CartItem> CartItems { get; }
        IRepository<Address> Addresses { get; }
        IRepository<Review> Reviews { get; }
        IRepository<WishList> WishLists { get; }
        IRepository<Contact> Contacts { get; }
        IRepository<BlogPost> BlogPosts { get; }

        /// <summary>
        /// Tüm değişiklikleri veritabanına kaydeder.
        /// 
        /// Bu metod, tüm repository'lerde yapılan değişiklikleri 
        /// (Add, Update, Delete) tek seferde veritabanına yazar.
        /// 
        /// ÖNEMLİ:
        /// - AddAsync, UpdateAsync, DeleteAsync çağrıldıktan sonra 
        ///   mutlaka bu metod çağrılmalıdır
        /// - SaveChanges çağrılmadan değişiklikler kalıcı olmaz
        /// 
        /// Dönüş Değeri:
        /// - Etkilenen kayıt sayısı (int)
        /// 
        /// Kullanım:
        /// await _unitOfWork.Products.AddAsync(product);
        /// await _unitOfWork.Categories.AddAsync(category);
        /// var affectedRows = await _unitOfWork.SaveChangesAsync();
        /// 
        /// NOT: Transaction içindeyse, commit edilene kadar değişiklikler 
        /// geçici olarak tutulur.
        /// </summary>
        /// <summary>
        /// Transaction (işlem) başlatır.
        /// 
        /// Transaction, birden fazla veritabanı işleminin birlikte 
        /// başarılı/başarısız olmasını sağlar (ACID özellikleri).
        /// 
        /// Kullanım Senaryosu:
        /// - Sipariş oluştururken hem Order hem OrderDetail kaydedilmeli
        /// - Eğer OrderDetail kaydedilemezse Order da kaydedilmemeli
        /// - Transaction bu tutarlılığı sağlar
        /// 
        /// ÖNEMLİ:
        /// - BeginTransactionAsync() çağrıldıktan sonra mutlaka 
        ///   CommitTransactionAsync() veya RollbackTransactionAsync() çağrılmalıdır
        /// - Zaten aktif bir transaction varsa yeni bir tane açılmaz
        /// 
        /// Kullanım:
        /// await _unitOfWork.BeginTransactionAsync();
        /// try {
        ///     // İşlemler
        ///     await _unitOfWork.SaveChangesAsync();
        ///     await _unitOfWork.CommitTransactionAsync();
        /// } catch {
        ///     await _unitOfWork.RollbackTransactionAsync();
        /// }
        /// </summary>
        /// <summary>
        /// Transaction'ı onaylar (commit eder).
        /// 
        /// Bu metod, transaction içindeki tüm değişiklikleri kalıcı hale getirir.
        /// Commit edildikten sonra değişiklikler geri alınamaz.
        /// 
        /// ÖNEMLİ:
        /// - BeginTransactionAsync() çağrıldıktan sonra kullanılmalıdır
        /// - SaveChangesAsync() commit'ten önce çağrılmalıdır
        /// - Commit edildikten sonra transaction kapatılır
        /// 
        /// Kullanım:
        /// await _unitOfWork.BeginTransactionAsync();
        /// await _unitOfWork.Products.AddAsync(product);
        /// await _unitOfWork.SaveChangesAsync();
        /// await _unitOfWork.CommitTransactionAsync(); // ŞİMDİ kalıcı oldu
        /// 
        /// NOT: Transaction yoksa hiçbir şey yapılmaz (sessizce başarısız olur).
        /// </summary>
        /// <summary>
        /// Transaction'ı geri alır (rollback eder).
        /// 
        /// Bu metod, transaction içindeki tüm değişiklikleri iptal eder.
        /// Hata durumunda kullanılır.
        /// 
        /// Kullanım Senaryosu:
        /// - Bir işlem başarısız olursa tüm değişiklikler geri alınır
        /// - Veritabanı tutarlılığı korunur
        /// 
        /// ÖNEMLİ:
        /// - BeginTransactionAsync() çağrıldıktan sonra kullanılmalıdır
        /// - Rollback edildikten sonra transaction kapatılır
        /// 
        /// Kullanım:
        /// await _unitOfWork.BeginTransactionAsync();
        /// try {
        ///     // İşlemler
        ///     await _unitOfWork.SaveChangesAsync();
        ///     await _unitOfWork.CommitTransactionAsync();
        /// } catch (Exception ex) {
        ///     await _unitOfWork.RollbackTransactionAsync(); // Tüm değişiklikler iptal
        ///     // Hata loglama
        /// }
        /// 
        /// NOT: Transaction yoksa hiçbir şey yapılmaz (sessizce başarısız olur).
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}



