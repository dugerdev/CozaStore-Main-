using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace CozaStore.Application.Services;

/// <summary>
/// Stripe ödeme işlemleri için servis
/// </summary>
public class PaymentService
{
    private readonly IConfiguration _configuration;

    public PaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
        // Stripe API key'ini ayarla
        var secretKey = _configuration["Stripe:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Stripe SecretKey bulunamadı. appsettings.json'da Stripe:SecretKey ayarını kontrol edin.");
        }
        StripeConfiguration.ApiKey = secretKey;
    }

    /// <summary>
    /// Stripe Checkout Session oluşturur
    /// </summary>
    /// <param name="amount">Ödeme tutarı (cent cinsinden, örn: 1000 = $10.00)</param>
    /// <param name="currency">Para birimi (varsayılan: "usd")</param>
    /// <param name="orderId">Sipariş ID'si</param>
    /// <param name="customerEmail">Müşteri e-posta adresi</param>
    /// <returns>Checkout Session URL'i</returns>
    public async Task<string> CreateCheckoutSessionAsync(
        long amount, 
        string currency = "usd", 
        string? orderId = null,
        string? customerEmail = null)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = amount, // Cent cinsinden (1000 = $10.00)
                        Currency = currency,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = orderId != null ? $"Order #{orderId}" : "Order Payment",
                            Description = "CozaStore Order Payment"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment", // "payment" = tek seferlik ödeme, "subscription" = abonelik
            SuccessUrl = "https://localhost:7002/Checkout/Success?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = "https://localhost:7002/Checkout/Cancel",
            Metadata = new Dictionary<string, string>()
        };

        // Sipariş ID'si varsa metadata'ya ekle
        if (!string.IsNullOrEmpty(orderId))
        {
            options.Metadata.Add("orderId", orderId);
        }

        // Müşteri e-posta adresi varsa ekle
        if (!string.IsNullOrEmpty(customerEmail))
        {
            options.CustomerEmail = customerEmail;
        }

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        if (string.IsNullOrEmpty(session.Url))
        {
            throw new InvalidOperationException("Stripe checkout session URL oluşturulamadı.");
        }

        return session.Url; // Kullanıcıyı bu URL'e yönlendireceğiz
    }

    /// <summary>
    /// Checkout Session'ı ID ile kontrol eder (ödeme başarılı mı?)
    /// </summary>
    public async Task<Session> GetCheckoutSessionAsync(string sessionId)
    {
        var service = new SessionService();
        return await service.GetAsync(sessionId);
    }
}
