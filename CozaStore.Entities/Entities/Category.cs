using CozaStore.Entities.Common;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Kategori (Category) entity'si.
    ///
    /// E-ticaret sisteminde ürünlerin sınıflandırılması için kullanılır.
    /// Template'de Women, Men, Shoes, Watches, Bag gibi kategoriler bulunur.
    ///
    /// BaseEntity'den türer, bu sayede:
    /// - Id (Primary Key)
    /// - CreatedDate, UpdatedDate (Audit alanları)
    /// - IsDeleted, IsActive (Soft Delete alanları)
    /// gibi ortak özellikleri otomatik olarak alır.
    ///
    /// İlişkiler:
    /// - Products: Bir kategoriye birden fazla ürün ait olabilir (One-to-Many)
    ///
    /// Kullanım Senaryosu:
    /// - Ürünler kategorilere göre gruplandırılır
    /// - Kategori sayfasında o kategoriye ait ürünler gösterilir
    /// - Navigasyon menüsünde kategoriler listelenir
    /// </summary>
    public class Category : BaseEntity
    {
        /// <summary>
        /// Kategori adı.
        ///
        /// Zorunlu alan. Kullanıcıya gösterilen kategori ismidir.
        /// Örnek: "Women", "Men", "Shoes", "Watches", "Bag"
        ///
        /// Validasyon: FluentValidation ile kontrol edilir (boş olamaz, max uzunluk vb.)
        ///
        /// Kullanım:
        /// - Navigasyon menüsünde gösterilir
        /// - URL'de kullanılabilir (SEO için)
        /// - Ürün listeleme sayfasında filtreleme için kullanılır
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Kategori açıklaması.
        ///
        /// Opsiyonel alan. Kategori hakkında detaylı bilgi içerir.
        /// HTML formatında olabilir (zengin metin editörü kullanılıyorsa).
        ///
        /// Örnek: "Kadın giyim ürünleri, moda ve stil..."
        ///
        /// Kullanım:
        /// - Kategori detay sayfasında gösterilir
        /// - SEO için önemlidir (meta description)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Kategori görseli URL'i.
        ///
        /// Opsiyonel alan. Kategorinin görselinin dosya yolu veya URL'i.
        ///
        /// Kullanım:
        /// - Yerel dosya: "/images/categories/women.jpg"
        /// - CDN URL: "https://cdn.example.com/categories/women.jpg"
        /// - Blob Storage: "https://storage.azure.com/container/category.jpg"
        ///
        /// Template'de bu görsel kategori kartlarında ve navigasyon menüsünde kullanılır.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Bu kategoriye ait ürünler.
        ///
        /// Navigation Property: EF Core'un ilişkileri yönetmesi için kullanılır.
        ///
        /// İlişki: One-to-Many (Bir kategori - Çok ürün)
        /// - Category.Products → Bu kategoriye ait tüm ürünleri getirir
        /// - Product.Category → Bu ürünün kategorisini getirir
        ///
        /// Kullanım:
        /// var category = await _context.Categories
        ///     .Include(c => c.Products)  // Lazy loading yerine eager loading
        ///     .FirstOrDefaultAsync();
        ///
        /// var productCount = category.Products.Count(); // Bu kategorideki ürün sayısı
        ///
        /// NOT: ICollection kullanıyoruz çünkü birden fazla Product olabilir.
        /// </summary>
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
