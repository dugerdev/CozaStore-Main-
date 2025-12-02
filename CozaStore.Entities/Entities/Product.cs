using CozaStore.Entities.Common;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Ürün (Product) entity'si.
    /// 
    /// E-ticaret sisteminin en temel varlığıdır. Tüm ürün bilgilerini 
    /// (ad, fiyat, stok, görsel vb.) bu entity'de tutarız.
    /// 
    /// BaseEntity'den türer, bu sayede:
    /// - Id (Primary Key)
    /// - CreatedDate, UpdatedDate (Audit alanları)
    /// - IsDeleted, IsActive (Soft Delete alanları)
    /// gibi ortak özellikleri otomatik olarak alır.
    /// 
    /// İlişkiler:
    /// - Category: Her ürün bir kategoriye aittir (Many-to-One)
    /// - OrderDetails: Bir ürün birden fazla sipariş detayında olabilir (One-to-Many)
    /// - CartItems: Bir ürün birden fazla sepette olabilir (One-to-Many)
    /// - Reviews: Bir ürüne birden fazla yorum yapılabilir (One-to-Many)
    /// - WishLists: Bir ürün birden fazla istek listesinde olabilir (One-to-Many)
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Ürün adı.
        /// 
        /// Zorunlu alan. Kullanıcıya gösterilen ürün ismidir.
        /// Örnek: "Nike Air Max", "iPhone 15 Pro", "Laptop Dell XPS"
        /// 
        /// Validasyon: FluentValidation ile kontrol edilir (boş olamaz, max uzunluk vb.)
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Ürün açıklaması.
        /// 
        /// Opsiyonel alan. Ürün hakkında detaylı bilgi içerir.
        /// HTML formatında olabilir (zengin metin editörü kullanılıyorsa).
        /// 
        /// Örnek: "Yüksek kaliteli deri ayakkabı, rahat ve şık tasarım..."
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Ürün fiyatı.
        /// 
        /// Zorunlu alan. Decimal formatında tutulur çünkü:
        /// - Para birimi hassasiyeti için önemlidir
        /// - Kuruş (cent) bilgisi korunur
        /// - Double/float yerine decimal kullanılır (hassasiyet için)
        /// 
        /// Örnek: 1500.99 TL, 29.99 USD
        /// 
        /// Validasyon: 0'dan büyük olmalıdır.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// İndirimli fiyat.
        /// 
        /// Opsiyonel alan. Ürün indirimdeyse bu alan doldurulur.
        /// Nullable çünkü her ürün indirimde olmayabilir.
        /// 
        /// Kullanım:
        /// - Normal fiyat: Price = 1000, DiscountPrice = null
        /// - İndirimli: Price = 1000, DiscountPrice = 750
        /// 
        /// Frontend'de indirim yüzdesi hesaplanabilir:
        /// İndirim % = ((Price - DiscountPrice) / Price) * 100
        /// </summary>
        public decimal? DiscountPrice { get; set; }

        /// <summary>
        /// Ana ürün görseli URL'i.
        /// 
        /// Opsiyonel alan. Ürünün ana görselinin dosya yolu veya URL'i.
        /// 
        /// Kullanım:
        /// - Yerel dosya: "/images/products/laptop-001.jpg"
        /// - CDN URL: "https://cdn.example.com/products/laptop-001.jpg"
        /// - Blob Storage: "https://storage.azure.com/container/product.jpg"
        /// 
        /// Template'de bu görsel ürün listesi ve detay sayfasında kullanılır.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Stock Keeping Unit (SKU) - Ürün stok kodu.
        /// 
        /// Opsiyonel alan. Ürünün benzersiz stok kodudur.
        /// Genellikle mağaza yönetim sistemlerinde kullanılır.
        /// 
        /// Örnek: "JAK-01", "LAPTOP-DELL-001", "PHONE-IPHONE-15-256GB"
        /// 
        /// Kullanım:
        /// - Stok takibi
        /// - Sipariş yönetimi
        /// - Envanter kontrolü
        /// </summary>
        public string? SKU { get; set; }

        /// <summary>
        /// Stok miktarı.
        /// 
        /// Ürünün mevcut stok adedini tutar.
        /// Varsayılan değer: 0 (stokta yok)
        /// 
        /// Kullanım:
        /// - Sipariş verilirken stok kontrolü yapılır
        /// - Stok bitince ürün "Stokta Yok" olarak işaretlenir
        /// - Negatif olamaz (validasyon ile kontrol edilir)
        /// 
        /// İş Kuralı:
        /// - StockQuantity > 0 → IsInStock = true
        /// - StockQuantity = 0 → IsInStock = false
        /// </summary>
        public int StockQuantity { get; set; } = 0;

        /// <summary>
        /// Stokta var mı?
        /// 
        /// Bu alan, ürünün stokta olup olmadığını belirtir.
        /// Hesaplanan bir değer olabilir ama performans için 
        /// veritabanında saklanır (cache).
        /// 
        /// Kullanım:
        /// - true: Ürün stokta, sipariş verilebilir
        /// - false: Ürün stokta yok, sipariş verilemez
        /// 
        /// Varsayılan değer: true
        /// 
        /// NOT: Bu alan StockQuantity'ye göre otomatik güncellenebilir.
        /// </summary>
        public bool IsInStock { get; set; } = true;

        /// <summary>
        /// Foreign Key - Kategori ID'si.
        /// 
        /// Bu ürünün hangi kategoriye ait olduğunu belirtir.
        /// Category entity'sine referans verir.
        /// 
        /// İlişki: Many-to-One (Çok ürün - Bir kategori)
        /// - Bir kategoriye birden fazla ürün ait olabilir
        /// - Her ürün sadece bir kategoriye ait olabilir
        /// 
        /// Zorunlu alan. Her ürünün mutlaka bir kategorisi olmalıdır.
        /// 
        /// Örnek: "Electronics", "Clothing", "Shoes"
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Ürünün kategorisi.
        /// 
        /// Navigation Property: EF Core'un ilişkileri yönetmesi için kullanılır.
        /// 
        /// İlişki: Many-to-One (Çok ürün - Bir kategori)
        /// - Product.Category → Bu ürünün kategorisini getirir
        /// - Category.Products → Bu kategoriye ait tüm ürünleri getirir
        /// 
        /// Kullanım:
        /// var product = await _context.Products
        ///     .Include(p => p.Category)  // Lazy loading yerine eager loading
        ///     .FirstOrDefaultAsync();
        /// 
        /// Console.WriteLine(product.Category.Name); // "Electronics"
        /// </summary>
        public Category? Category { get; set; }

        /// <summary>
        /// Ürünün sipariş detayları.
        /// 
        /// Navigation Property: Bu ürünün hangi siparişlerde yer aldığını gösterir.
        /// 
        /// İlişki: One-to-Many (Bir ürün - Çok sipariş detayı)
        /// - Bir ürün birden fazla siparişte olabilir
        /// - Her sipariş detayı bir ürüne aittir
        /// 
        /// Kullanım:
        /// var product = await _context.Products
        ///     .Include(p => p.OrderDetails)
        ///     .FirstOrDefaultAsync();
        /// 
        /// var totalSold = product.OrderDetails.Sum(od => od.Quantity);
        /// 
        /// NOT: ICollection kullanıyoruz çünkü birden fazla OrderDetail olabilir.
        /// </summary>
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        /// <summary>
        /// Sepetteki bu ürünün öğeleri.
        /// 
        /// Navigation Property: Bu ürünün hangi kullanıcıların 
        /// sepetinde olduğunu gösterir.
        /// 
        /// İlişki: One-to-Many (Bir ürün - Çok sepet öğesi)
        /// - Bir ürün birden fazla kullanıcının sepetinde olabilir
        /// - Her sepet öğesi bir ürüne aittir
        /// 
        /// Kullanım:
        /// - Sepet sayfasında ürün bilgilerini göstermek için
        /// - Stok kontrolü yaparken sepetteki miktarı hesaplamak için
        /// </summary>
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        /// <summary>
        /// Ürüne yapılan yorumlar/değerlendirmeler.
        /// 
        /// Navigation Property: Bu ürüne yapılan tüm yorumları gösterir.
        /// 
        /// İlişki: One-to-Many (Bir ürün - Çok yorum)
        /// - Bir ürüne birden fazla yorum yapılabilir
        /// - Her yorum bir ürüne aittir
        /// 
        /// Kullanım:
        /// - Ürün detay sayfasında yorumları göstermek için
        /// - Ortalama puan hesaplamak için
        /// - Yorum sayısını göstermek için
        /// 
        /// NOT: Sadece onaylanmış yorumlar (IsApproved = true) gösterilir.
        /// </summary>
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        /// <summary>
        /// İstek listelerindeki bu ürünün kayıtları.
        /// 
        /// Navigation Property: Bu ürünün hangi kullanıcıların 
        /// istek listesinde olduğunu gösterir.
        /// 
        /// İlişki: One-to-Many (Bir ürün - Çok istek listesi kaydı)
        /// - Bir ürün birden fazla kullanıcının istek listesinde olabilir
        /// - Her istek listesi kaydı bir ürüne aittir
        /// 
        /// Kullanım:
        /// - "Bu ürünü X kişi beğendi" gibi istatistikler için
        /// - Popüler ürünleri bulmak için
        /// </summary>
        public ICollection<WishList> WishLists { get; set; } = new List<WishList>();
    }
}
