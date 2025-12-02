namespace CozaStore.Entities.Enums
{
    /// <summary>
    /// Ödeme durumu enum'u. Siparişin ödeme durumunu temsil eder.
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Ödenmedi. Sipariş henüz ödenmedi.
        /// </summary>
        Unpaid = 0,

        /// <summary>
        /// Ödendi. Sipariş ödendi.
        /// </summary>
        Paid = 1,

        /// <summary>
        /// İade edildi. Ödeme iade edildi.
        /// </summary>
        Refunded = 2,

        /// <summary>
        /// Kısmen iade edildi. Ödeme kısmen iade edildi.
        /// </summary>
        PartiallyRefunded = 3
    }
}
