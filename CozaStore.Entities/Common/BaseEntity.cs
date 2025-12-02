// Using Directives
using System;

namespace CozaStore.Entities.Common
{
    // BaseEntity Class - Açıklamalar
    /// <summary>
    /// Tüm entity'ler için ortak base (temel) sınıf.
    /// 
    /// Bu sınıf, projedeki tüm entity'lerin (Product, Category, Order vb.) 
    /// ortak özelliklerini içerir. Bu sayede kod tekrarını önleriz ve 
    /// tutarlılık sağlarız.
    /// 
    /// Özellikler:
    /// - Primary Key (Id): Her kayıt için benzersiz GUID
    /// - Audit Fields: Oluşturulma, güncellenme, silinme tarihleri
    /// - Soft Delete: Fiziksel silme yerine mantıksal silme
    /// - IsActive: Kayıt aktif/pasif durumu
    /// 
    /// Kullanım:
    /// public class Product : BaseEntity { ... }
    /// public class Category : BaseEntity { ... }
    /// </summary>
    public abstract class BaseEntity
    {
        // Primary Key - Açıklamalar
        /// <summary>
        /// Primary Key (Birincil Anahtar).
        /// 
        /// Her kayıt için benzersiz bir kimlik sağlar.
        /// GUID (Globally Unique Identifier) kullanıyoruz çünkü:
        /// - Distributed sistemlerde çakışma riski yok
        /// - Güvenli (tahmin edilemez)
        /// - Veritabanına bağımlı değil (auto-increment gibi)
        /// 
        /// Varsayılan değer: Yeni GUID oluşturulur (Guid.NewGuid())
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        // Audit Fields (Denetim Alanları) - Açıklamalar
        /// <summary>
        /// Kayıt oluşturulma tarihi.
        /// 
        /// Bu alan, kaydın ne zaman oluşturulduğunu tutar.
        /// UTC (Coordinated Universal Time) formatında saklanır çünkü:
        /// - Farklı zaman dilimlerinde tutarlılık sağlar
        /// - Daylight Saving Time (Yaz Saati) sorunlarını önler
        /// - Sunucu zamanına göre çalışır
        /// 
        /// Varsayılan değer: Şu anki UTC zamanı (DateTime.UtcNow)
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Kayıt güncellenme tarihi.
        /// 
        /// Bu alan, kaydın en son ne zaman güncellendiğini tutar.
        /// Nullable (DateTime?) çünkü:
        /// - Yeni oluşturulan kayıtlar henüz güncellenmemiştir
        /// - Sadece güncelleme yapıldığında doldurulur
        /// 
        /// Kullanım:
        /// - Kayıt oluşturulduğunda: null
        /// - Kayıt güncellendiğinde: Güncelleme zamanı
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        // Soft Delete Fields (Mantıksal Silme Alanları) - Açıklamalar
        /// <summary>
        /// Kayıt aktif mi?
        /// 
        /// Bu alan, kaydın aktif/pasif durumunu belirtir.
        /// Soft delete mekanizmasının bir parçasıdır.
        /// 
        /// Kullanım:
        /// - true: Kayıt aktif, kullanılabilir
        /// - false: Kayıt pasif, kullanılamaz (silinmiş gibi)
        /// 
        /// Varsayılan değer: true (yeni kayıtlar aktif olarak başlar)
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Soft Delete (Mantıksal Silme) için kullanılan alan.
        /// 
        /// Hard Delete (Fiziksel Silme) yerine Soft Delete kullanıyoruz çünkü:
        /// - Veri kaybını önleriz (geri getirme imkanı)
        /// - Audit (denetim) için önemlidir
        /// - İlişkili kayıtların bütünlüğünü koruruz
        /// - Yasal gereklilikler (GDPR vb.) için önemli
        /// 
        /// Kullanım:
        /// - false: Kayıt normal, sorgularda görünür
        /// - true: Kayıt silinmiş sayılır, sorgularda görünmez
        /// 
        /// Varsayılan değer: false (yeni kayıtlar silinmemiş olarak başlar)
        /// 
        /// NOT: Repository katmanında otomatik olarak IsDeleted = false 
        /// olan kayıtlar getirilir.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Kayıt silinme tarihi.
        /// 
        /// Bu alan, kaydın ne zaman silindiğini (soft delete) tutar.
        /// Nullable (DateTime?) çünkü:
        /// - Aktif kayıtlar henüz silinmemiştir
        /// - Sadece soft delete yapıldığında doldurulur
        /// 
        /// Kullanım:
        /// - Kayıt aktifken: null
        /// - Soft delete yapıldığında: Silinme zamanı (UTC)
        /// 
        /// Audit (denetim) için önemlidir.
        /// </summary>
        public DateTime? DeletedDate { get; set; }
    }
}
