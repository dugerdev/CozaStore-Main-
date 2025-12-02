using CozaStore.Entities.Common;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Sipariş detayı entity'si. Bir siparişteki ürün bilgilerini tutar.
    /// Bir sipariş birden fazla ürün içerebilir, her ürün için ayrı OrderDetail oluşturulur.
    /// </summary>
    public class OrderDetail : BaseEntity
    {
        /// <summary>
        /// Foreign Key - Hangi siparişe ait?
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Foreign Key - Hangi ürün?
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Ürün adı (sipariş anındaki). Ürün silinse veya değişse bile sipariş kaydında korunur.
        /// Bu alan önemli çünkü sipariş geçmişi tutarlı kalmalı.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Ürün fiyatı (sipariş anındaki). Ürün fiyatı değişse bile sipariş kaydında korunur.
        /// Bu alan önemli çünkü sipariş geçmişi tutarlı kalmalı.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Sipariş edilen miktar.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Bu satırın toplam tutarı (UnitPrice * Quantity).
        /// Hesaplanabilir ama performans için saklanabilir.
        /// </summary>
        public decimal SubTotal { get; set; }

        // Navigation Properties

        /// <summary>
        /// Bu detayın ait olduğu sipariş. Many-to-One ilişki.
        /// </summary>
        public Order Order { get; set; }

        /// <summary>
        /// Bu detayın ürünü. Many-to-One ilişki.
        /// </summary>
        public Product Product { get; set; }
    }
}
