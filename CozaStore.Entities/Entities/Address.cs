using CozaStore.Entities.Common;
using CozaStore.Entities.Enums;

namespace CozaStore.Entities.Entities
{
    /// <summary>
    /// Adres entity'si. Kullanıcının teslimat ve fatura adreslerini tutar.
    /// </summary>
    public class Address : BaseEntity
    {
        /// <summary>
        /// Kullanıcı ID'si. Bu adres hangi kullanıcıya ait?
        /// String çünkü Identity User ID'si string (Guid değil).
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Adres başlığı (örn: "Ev", "İş", "Anne Evi").
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Adres satırı 1 (Sokak, cadde, mahalle).
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Adres satırı 2 (Daire, kat, bina no - opsiyonel).
        /// </summary>
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Şehir.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// İlçe.
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// Posta kodu.
        /// </summary>
        public string? PostalCode { get; set; }

        /// <summary>
        /// Ülke.
        /// </summary>
        public string Country { get; set; } = "Turkey";

        /// <summary>
        /// Adres türü. Enum kullanarak type-safe hale getiriyoruz.
        /// </summary>
        public AddressType? AddressType { get; set; }

        /// <summary>
        /// Varsayılan adres mi? Kullanıcının birden fazla adresi olabilir.
        /// </summary>
        public bool IsDefault { get; set; } = false;
    }
}
