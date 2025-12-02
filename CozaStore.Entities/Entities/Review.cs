using CozaStore.Entities.Common;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Yorum/Değerlendirme entity'si. Kullanıcıların ürünlere yaptığı yorumları tutar.
    /// </summary>
    public class Review : BaseEntity
    {
        /// <summary>
        /// Foreign Key - Hangi ürüne yorum yapıldı?
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Kullanıcı ID'si. Kim yorum yaptı?
        /// String çünkü Identity User ID'si string (Guid değil).
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Yorum başlığı.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Yorum içeriği.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Puan (1-5 arası). 5 yıldız sistemi.
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Yorum onaylandı mı? Admin onayından sonra gösterilir.
        /// </summary>
        public bool IsApproved { get; set; } = false;

        // Navigation Properties

        /// <summary>
        /// Yorum yapılan ürün. Many-to-One ilişki.
        /// </summary>
        public Product Product { get; set; }
    }
}
