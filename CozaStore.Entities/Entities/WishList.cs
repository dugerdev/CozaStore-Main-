using CozaStore.Entities.Common;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// İstek listesi entity'si. Kullanıcının beğendiği ürünleri tutar.
    /// </summary>
    public class WishList : BaseEntity
    {
        /// <summary>
        /// Kullanıcı ID'si. Bu ürün hangi kullanıcının istek listesinde?
        /// String çünkü Identity User ID'si string (Guid değil).
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Foreign Key - Hangi ürün istek listesinde?
        /// </summary>
        public Guid ProductId { get; set; }

        // Navigation Properties

        /// <summary>
        /// İstek listesindeki ürün. Many-to-One ilişki.
        /// </summary>
        public Product Product { get; set; }
    }
}
