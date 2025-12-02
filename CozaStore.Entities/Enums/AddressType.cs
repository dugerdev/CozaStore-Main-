namespace CozaStore.Entities.Enums
{
    /// <summary>
    /// Adres türü enum'u. Adresin teslimat mı fatura mı olduğunu belirtir.
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Teslimat adresi. Ürünlerin gönderileceği adres.
        /// </summary>
        Shipping = 0,

        /// <summary>
        /// Fatura adresi. Faturanın gönderileceği adres.
        /// </summary>
        Billing = 1
    }
}
