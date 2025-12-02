using CozaStore.Entities.Common;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// İletişim mesajı entity'si. Kullanıcıların gönderdiği iletişim formu mesajlarını tutar.
    /// </summary>
    public class Contact : BaseEntity
    {
        /// <summary>
        /// Gönderenin email adresi.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gönderilen mesaj içeriği.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Mesaj okundu mu? Admin tarafından okunduğunda true olur.
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Mesaj okunma tarihi.
        /// </summary>
        public DateTime? ReadDate { get; set; }
    }
}


