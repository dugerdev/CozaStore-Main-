using CozaStore.Entities.Common;
using CozaStore.Entities.Enums;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Sipariş (Order) entity'si.
    ///
    /// Müşterinin verdiği sipariş bilgilerini tutar. Bir sipariş birden fazla
    /// ürün içerebilir (OrderDetail üzerinden).
    ///
    /// BaseEntity'den türer, bu sayede:
    /// - Id (Primary Key)
    /// - CreatedDate, UpdatedDate (Audit alanları)
    /// - IsDeleted, IsActive (Soft Delete alanları)
    /// gibi ortak özellikleri otomatik olarak alır.
    ///
    /// İlişkiler:
    /// - OrderDetails: Bir siparişte birden fazla ürün olabilir (One-to-Many)
    /// - ShippingAddress: Teslimat adresi (Many-to-One)
    /// - BillingAddress: Fatura adresi (Many-to-One, opsiyonel)
    ///
    /// İş Akışı:
    /// 1. Kullanıcı sepetten sipariş verir
    /// 2. Order oluşturulur (Status: Pending, PaymentStatus: Unpaid)
    /// 3. Ödeme yapılır (PaymentStatus: Paid)
    /// 4. Sipariş hazırlanır (Status: Processing)
    /// 5. Kargo edilir (Status: Shipped)
    /// 6. Teslim edilir (Status: Delivered)
    /// </summary>
    public class Order : BaseEntity
    {
        /// <summary>
        /// Sipariş numarası.
        ///
        /// Benzersiz, kullanıcıya gösterilen numara.
        /// Genellikle format: "ORD-YYYY-MMDD-HHMMSS" veya "ORD-2024-001"
        ///
        /// Kullanım:
        /// - Kullanıcı siparişini takip ederken bu numarayı kullanır
        /// - Müşteri hizmetleri siparişi bu numara ile bulur
        /// - Fatura ve kargo takip sisteminde kullanılır
        ///
        /// Örnek: "ORD-2024-1101-143022", "ORD-2024-001"
        ///
        /// Validasyon: Benzersiz olmalı, boş olamaz.
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Sipariş tarihi.
        ///
        /// Sipariş ne zaman verildi?
        /// UTC formatında saklanır (zaman dilimi sorunlarını önlemek için).
        ///
        /// Varsayılan değer: Şu anki UTC zamanı (DateTime.UtcNow)
        ///
        /// Kullanım:
        /// - Sipariş geçmişi listesinde gösterilir
        /// - Raporlama için kullanılır (günlük, aylık siparişler)
        /// - İade süresi hesaplaması için önemlidir
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Toplam tutar.
        ///
        /// Sipariş detaylarının toplamı + kargo + vergi.
        /// Decimal formatında tutulur (para birimi hassasiyeti için).
        ///
        /// Hesaplama:
        /// TotalAmount = (OrderDetails.Sum(od => od.SubTotal)) + ShippingCost + TaxAmount
        ///
        /// Örnek: 1500.99 TL
        ///
        /// Kullanım:
        /// - Sipariş özetinde gösterilir
        /// - Ödeme işleminde kullanılır
        /// - Fatura tutarı olarak kullanılır
        ///
        /// Validasyon: 0'dan büyük olmalıdır.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Kargo ücreti.
        ///
        /// Siparişin teslimat maliyeti.
        /// Varsayılan değer: 0 (ücretsiz kargo durumunda)
        ///
        /// Kullanım:
        /// - Belirli bir tutarın üzerindeki siparişlerde ücretsiz olabilir
        /// - Bölgeye göre değişebilir
        /// - Kargo şirketine göre değişebilir
        ///
        /// Örnek: 25.00 TL, 0.00 TL (ücretsiz kargo)
        /// </summary>
        public decimal ShippingCost { get; set; } = 0;

        /// <summary>
        /// Vergi tutarı (KDV vb.).
        ///
        /// Siparişe uygulanan vergi miktarı.
        /// Varsayılan değer: 0
        ///
        /// Kullanım:
        /// - KDV hesaplaması (örn: %20 KDV)
        /// - ÖTV (Özel Tüketim Vergisi) hesaplaması
        /// - Fatura düzenleme için önemlidir
        ///
        /// Hesaplama:
        /// TaxAmount = (SubTotal * TaxRate) / 100
        ///
        /// Örnek: 300.00 TL (KDV %20 ise 1500 TL üzerinden)
        /// </summary>
        public decimal TaxAmount { get; set; } = 0;

        /// <summary>
        /// Sipariş durumu.
        ///
        /// Enum kullanarak type-safe hale getiriyoruz.
        /// Veritabanında string olarak saklanır (daha okunabilir).
        ///
        /// Olası Değerler:
        /// - Pending: Beklemede (sipariş verildi, henüz işleme alınmadı)
        /// - Processing: İşleniyor (sipariş hazırlanıyor)
        /// - Shipped: Kargoya verildi
        /// - Delivered: Teslim edildi
        /// - Cancelled: İptal edildi
        /// - Returned: İade edildi
        ///
        /// Varsayılan değer: Pending
        ///
        /// Kullanım:
        /// - Sipariş takip sayfasında gösterilir
        /// - E-posta bildirimlerinde kullanılır
        /// - Admin panelinde sipariş yönetimi için kullanılır
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        /// <summary>
        /// Ödeme durumu.
        ///
        /// Enum kullanarak type-safe hale getiriyoruz.
        /// Veritabanında string olarak saklanır (daha okunabilir).
        ///
        /// Olası Değerler:
        /// - Unpaid: Ödenmedi
        /// - Paid: Ödendi
        /// - Refunded: İade edildi
        /// - PartiallyRefunded: Kısmen iade edildi
        ///
        /// Varsayılan değer: Unpaid
        ///
        /// Kullanım:
        /// - Ödeme kontrolü için kullanılır
        /// - Sipariş işleme akışında önemlidir (ödeme yapılmadan sipariş işlenmez)
        /// - Muhasebe raporlarında kullanılır
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        /// <summary>
        /// Ödeme yöntemi.
        ///
        /// Enum kullanarak type-safe hale getiriyoruz.
        /// Opsiyonel çünkü ödeme yapılmadan önce null olabilir.
        ///
        /// Olası Değerler:
        /// - CreditCard: Kredi kartı
        /// - DebitCard: Banka kartı
        /// - BankTransfer: Havale/EFT
        /// - CashOnDelivery: Kapıda ödeme
        /// - PayPal: PayPal
        /// - Other: Diğer
        ///
        /// Kullanım:
        /// - Ödeme işlemi sırasında seçilir
        /// - Fatura düzenleme için önemlidir
        /// - Raporlama için kullanılır (hangi ödeme yöntemi daha çok kullanılıyor?)
        /// </summary>
        public PaymentMethod? PaymentMethod { get; set; }

        /// <summary>
        /// Teslimat adresi ID'si.
        ///
        /// Foreign Key ile Address entity'sine bağlı.
        /// Zorunlu alan. Her siparişin mutlaka bir teslimat adresi olmalıdır.
        ///
        /// Kullanım:
        /// - Sipariş verilirken kullanıcının adreslerinden biri seçilir
        /// - Kargo şirketine teslimat adresi olarak iletilir
        /// - Sipariş takip sayfasında gösterilir
        /// </summary>
        public Guid ShippingAddressId { get; set; }

        /// <summary>
        /// Fatura adresi ID'si.
        ///
        /// Foreign Key ile Address entity'sine bağlı.
        /// Opsiyonel alan. Teslimat adresi ile aynı olabilir.
        ///
        /// Kullanım:
        /// - Fatura düzenleme için kullanılır
        /// - Şirket adresleri için önemlidir (farklı fatura adresi olabilir)
        /// - Null ise teslimat adresi kullanılır
        /// </summary>
        public Guid? BillingAddressId { get; set; }

        /// <summary>
        /// Kullanıcı ID'si.
        ///
        /// ASP.NET Core Identity'den gelen User ID.
        /// String çünkü Identity User ID'si string (Guid değil).
        ///
        /// Kullanım:
        /// - Siparişin hangi kullanıcıya ait olduğunu belirtir
        /// - Kullanıcının sipariş geçmişini getirmek için kullanılır
        /// - Raporlama için kullanılır (hangi kullanıcı ne kadar sipariş verdi?)
        ///
        /// NOT: Identity User ID'si Guid değil string'dir.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Sipariş notları.
        ///
        /// Opsiyonel alan. Kullanıcı veya admin tarafından eklenebilir.
        ///
        /// Kullanım:
        /// - Kullanıcı özel talimatlar ekleyebilir (örn: "Kapıyı çalma, zile bas")
        /// - Admin iç notlar ekleyebilir (örn: "Hızlı teslimat yapılacak")
        /// - Müşteri hizmetleri notları ekleyebilir
        ///
        /// Örnek: "Lütfen paketi dikkatli taşıyın", "Öğleden sonra teslim edin"
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Sipariş detayları.
        ///
        /// Navigation Property: Bu siparişteki tüm ürünleri gösterir.
        ///
        /// İlişki: One-to-Many (Bir sipariş - Çok sipariş detayı)
        /// - Bir siparişte birden fazla ürün olabilir
        /// - Her sipariş detayı bir siparişe aittir
        ///
        /// Kullanım:
        /// var order = await _context.Orders
        ///     .Include(o => o.OrderDetails)
        ///         .ThenInclude(od => od.Product)  // Ürün bilgilerini de getir
        ///     .FirstOrDefaultAsync();
        ///
        /// var totalItems = order.OrderDetails.Sum(od => od.Quantity);
        /// var totalAmount = order.OrderDetails.Sum(od => od.SubTotal);
        ///
        /// NOT: ICollection kullanıyoruz çünkü birden fazla OrderDetail olabilir.
        /// </summary>
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        /// <summary>
        /// Teslimat adresi.
        ///
        /// Navigation Property: Bu siparişin teslimat adresini gösterir.
        ///
        /// İlişki: Many-to-One (Çok sipariş - Bir adres)
        /// - Birden fazla sipariş aynı adrese gönderilebilir
        /// - Her siparişin bir teslimat adresi vardır
        ///
        /// Kullanım:
        /// var order = await _context.Orders
        ///     .Include(o => o.ShippingAddress)
        ///     .FirstOrDefaultAsync();
        ///
        /// Console.WriteLine(order.ShippingAddress.AddressLine1);
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// Fatura adresi.
        ///
        /// Navigation Property: Bu siparişin fatura adresini gösterir.
        ///
        /// İlişki: Many-to-One (Çok sipariş - Bir adres)
        /// - Birden fazla sipariş aynı fatura adresine sahip olabilir
        /// - Her siparişin bir fatura adresi olabilir (opsiyonel)
        ///
        /// Kullanım:
        /// var order = await _context.Orders
        ///     .Include(o => o.BillingAddress)
        ///     .FirstOrDefaultAsync();
        ///
        /// if (order.BillingAddress != null)
        /// {
        ///     Console.WriteLine(order.BillingAddress.AddressLine1);
        /// }
        /// </summary>
        public Address? BillingAddress { get; set; }
    }
}
