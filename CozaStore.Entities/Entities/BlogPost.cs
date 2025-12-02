using CozaStore.Entities.Common;
using CozaStore.Entities.Identity;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Blog gönderisi (Blog Post) entity'si.
    /// 
    /// Admin kullanıcılarının paylaştığı blog gönderilerini tutar.
    /// Fotoğraflı gönderiler, yeni sezon ürün tanıtımları vb. için kullanılır.
    /// 
    /// BaseEntity'den türer, bu sayede:
    /// - Id (Primary Key)
    /// - CreatedDate, UpdatedDate (Audit alanları)
    /// - IsDeleted, IsActive (Soft Delete alanları)
    /// gibi ortak özellikleri otomatik olarak alır.
    /// 
    /// İlişkiler:
    /// - Author: Her blog gönderisi bir kullanıcı tarafından oluşturulur (Many-to-One)
    /// </summary>
    public class BlogPost : BaseEntity
    {
        /// <summary>
        /// Blog gönderisi başlığı.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Blog gönderisi içeriği.
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// Blog gönderisi görsel URL'i.
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Blog gönderisi yayınlandı mı?
        /// Sadece yayınlanan gönderiler public sayfada görünür.
        /// </summary>
        public bool IsPublished { get; set; } = false;

        /// <summary>
        /// Foreign Key - Hangi kullanıcı bu gönderiyi oluşturdu?
        /// Guid çünkü ApplicationUser.Id Guid tipinde (IdentityUser&lt;Guid&gt;).
        /// </summary>
        public required Guid AuthorId { get; set; }

        // Navigation Properties

        /// <summary>
        /// Blog gönderisini oluşturan kullanıcı. Many-to-One ilişki.
        /// </summary>
        public ApplicationUser? Author { get; set; }
    }
}
