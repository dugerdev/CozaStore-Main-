namespace CozaStore.Entities.Enums
{
    /// <summary>
    /// Sipariş durumu enum'u. Bir siparişin hayat döngüsündeki durumlarını temsil eder.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Beklemede. Sipariş verildi ama henüz işleme alınmadı.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// İşleniyor. Sipariş hazırlanıyor, paketleniyor.
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Kargoya verildi. Sipariş kargoya teslim edildi.
        /// </summary>
        Shipped = 2,

        /// <summary>
        /// Teslim edildi. Sipariş müşteriye ulaştı.
        /// </summary>
        Delivered = 3,

        /// <summary>
        /// İptal edildi. Sipariş iptal edildi.
        /// </summary>
        Cancelled = 4
    }
}
