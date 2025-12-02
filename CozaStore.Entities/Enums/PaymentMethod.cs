namespace CozaStore.Entities.Enums
{
    /// <summary>
    /// Ödeme yöntemi enum'u. Müşterinin seçtiği ödeme yöntemini temsil eder.
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Kredi kartı ile ödeme.
        /// </summary>
        CreditCard = 0,

        /// <summary>
        /// Banka transferi ile ödeme.
        /// </summary>
        BankTransfer = 1,

        /// <summary>
        /// Kapıda ödeme (Nakit veya kredi kartı).
        /// </summary>
        CashOnDelivery = 2,

        /// <summary>
        /// Dijital cüzdan ile ödeme (PayPal, PayTR vb.).
        /// </summary>
        DigitalWallet = 3
    }
}
