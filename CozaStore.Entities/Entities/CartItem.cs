using CozaStore.Entities.Common;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Sepet öğesi entity'si. Kullanıcının sepete eklediği ürünleri tutar.
    /// </summary>
    public class CartItem : BaseEntity
    {
        /// <summary>
        /// Kullanıcı ID'si. ASP.NET Core Identity'den gelen User ID.
        /// String çünkü Identity User ID'si string (Guid değil).
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Foreign Key - Hangi ürün sepette?
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Sepete eklenen miktar.
        /// </summary>
        public int Quantity { get; set; } = 1;

        // Navigation Properties

        /// <summary>
        /// Sepetteki ürün. Many-to-One ilişki.
        /// </summary>
        public Product Product { get; set; }
    }
}
