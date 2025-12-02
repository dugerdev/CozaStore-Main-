using CozaStore.Application.DTOs;
using CozaStore.Application.Enums;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CozaStore.WebUI.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly CartService _cartService;
        private readonly AddressService _addressService;
        private readonly OrderService _orderService;
        private readonly PaymentService _paymentService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            CartService cartService,
            AddressService addressService,
            OrderService orderService,
            PaymentService paymentService,
            ILogger<CheckoutController> logger)
        {
            _cartService = cartService;
            _addressService = addressService;
            _orderService = orderService;
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Checkout sayfasını gösterir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Sepeti kontrol et
            var cartResult = await _cartService.GetMyCartAsync();
            var cartItems = cartResult?.IsSuccess == true && cartResult.Data != null
                ? cartResult.Data
                : new List<CartItemDto>();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Sepetiniz boş. Lütfen önce ürün ekleyin.";
                return RedirectToAction("Index", "Cart");
            }

            // Kullanıcının adreslerini getir
            var addressesResult = await _addressService.GetMyAddressesAsync();
            var addresses = addressesResult?.IsSuccess == true && addressesResult.Data != null
                ? addressesResult.Data
                : new List<AddressDto>();

            var total = cartItems.Sum(item => item.SubTotal);

            ViewData["CartItems"] = cartItems;
            ViewData["Total"] = total;
            ViewData["Addresses"] = addresses;
            ViewData["Title"] = "Checkout";
            ViewData["ActiveMenu"] = "Cart";
            ViewData["HeaderClass"] = "header-v4";
            ViewData["MenuShadow"] = "yes";

            return View();
        }

        /// <summary>
        /// Yeni adres kaydeder
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddress(
            string title,
            string addressLine1,
            string? addressLine2,
            string city,
            string district,
            string? postalCode,
            string country)
        {
            if (string.IsNullOrWhiteSpace(title) || 
                string.IsNullOrWhiteSpace(addressLine1) || 
                string.IsNullOrWhiteSpace(city) || 
                string.IsNullOrWhiteSpace(district))
            {
                return Json(new { success = false, message = "Lütfen tüm zorunlu alanları doldurun." });
            }

            // Kullanıcı ID'sini al
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Kullanıcı bilgisi bulunamadı." });
            }

            // Yeni adres DTO oluştur (AddressService AddressDto bekliyor)
            var newAddress = new AddressDto(
                Id: Guid.NewGuid(), // API tarafında yeni ID oluşturulacak
                UserId: userId,
                Title: title,
                AddressLine1: addressLine1,
                AddressLine2: addressLine2,
                City: city,
                District: district,
                PostalCode: postalCode,
                Country: country ?? "Turkey",
                AddressType: null, // Varsayılan olarak null
                IsDefault: false
            );

            // Adresi kaydet
            _logger.LogInformation("Attempting to save new address. UserId: {UserId}, Title: {Title}", userId, title);
            var result = await _addressService.CreateAddressAsync(newAddress);

            if (result != null && result.IsSuccess)
            {
                _logger.LogInformation("New address saved successfully. Title: {Title}", title);
                return Json(new { success = true, message = "Adres başarıyla kaydedildi." });
            }

            var errorMessage = result?.ErrorMessage ?? "Adres kaydedilemedi. Lütfen tekrar deneyin.";
            _logger.LogError("Address save failed. Error: {Error}, UserId: {UserId}, Result IsSuccess: {IsSuccess}", 
                errorMessage, userId, result?.IsSuccess ?? false);
            return Json(new { success = false, message = errorMessage });
        }

        /// <summary>
        /// Adresi günceller
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress(
            Guid id,
            string title,
            string addressLine1,
            string? addressLine2,
            string city,
            string district,
            string? postalCode,
            string country)
        {
            if (id == Guid.Empty)
            {
                return Json(new { success = false, message = "Geçersiz adres ID." });
            }

            if (string.IsNullOrWhiteSpace(title) || 
                string.IsNullOrWhiteSpace(addressLine1) || 
                string.IsNullOrWhiteSpace(city) || 
                string.IsNullOrWhiteSpace(district))
            {
                return Json(new { success = false, message = "Lütfen tüm zorunlu alanları doldurun." });
            }

            // Kullanıcı ID'sini al
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Kullanıcı bilgisi bulunamadı." });
            }

            // Güncellenmiş adres DTO oluştur
            var updatedAddress = new AddressDto(
                Id: id,
                UserId: userId,
                Title: title,
                AddressLine1: addressLine1,
                AddressLine2: addressLine2,
                City: city,
                District: district,
                PostalCode: postalCode,
                Country: country ?? "Turkey",
                AddressType: null,
                IsDefault: false
            );

            // Adresi güncelle
            _logger.LogInformation("Attempting to update address. ID: {AddressId}, Title: {Title}", id, title);
            var result = await _addressService.UpdateAddressAsync(id, updatedAddress);

            if (result != null && result.IsSuccess)
            {
                _logger.LogInformation("Address updated successfully. ID: {AddressId}", id);
                return Json(new { success = true, message = "Adres başarıyla güncellendi." });
            }

            var errorMessage = result?.ErrorMessage ?? "Adres güncellenemedi.";
            _logger.LogError("Address update failed. ID: {AddressId}, Error: {Error}", id, errorMessage);
            return Json(new { success = false, message = errorMessage });
        }

        /// <summary>
        /// Adresi siler
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Json(new { success = false, message = "Geçersiz adres ID." });
            }

            var result = await _addressService.DeleteAddressAsync(id);

            if (result != null && result.IsSuccess)
            {
                _logger.LogInformation("Address deleted. ID: {AddressId}", id);
                return Json(new { success = true, message = "Adres başarıyla silindi." });
            }

            var errorMessage = result?.ErrorMessage ?? "Adres silinemedi.";
            _logger.LogWarning("Address deletion failed. Error: {Error}", errorMessage);
            return Json(new { success = false, message = errorMessage });
        }

        /// <summary>
        /// Stripe Checkout Session oluşturur ve kullanıcıyı Stripe'a yönlendirir
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePayment(
            Guid shippingAddressId,
            Guid? billingAddressId)
        {
            // Çok görünür log - her zaman çalışmalı
            System.Console.WriteLine("=== CreatePayment METHOD CALLED ===");
            _logger.LogError("=== CreatePayment METHOD CALLED ===");
            _logger.LogInformation("CreatePayment called. ShippingAddressId: {ShippingAddressId}, BillingAddressId: {BillingAddressId}", 
                shippingAddressId, billingAddressId);
            try
            {
                // Sepeti kontrol et
                var cartResult = await _cartService.GetMyCartAsync();
                var cartItems = cartResult?.IsSuccess == true && cartResult.Data != null
                    ? cartResult.Data
                    : new List<CartItemDto>();

                if (!cartItems.Any())
                {
                    TempData["ErrorMessage"] = "Sepetiniz boş.";
                    return RedirectToAction("Index", "Cart");
                }

                // Validasyon
                if (shippingAddressId == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Lütfen teslimat adresi seçin.";
                    return RedirectToAction("Index");
                }

                // Önce siparişi oluştur (Pending durumunda)
                _logger.LogInformation("Creating order...");
                var orderDetails = cartItems.Select(item => new OrderItemRequestDto(
                    item.ProductId,
                    item.Quantity
                )).ToList();

                var total = cartItems.Sum(item => item.SubTotal);
                var createOrderRequest = new CreateOrderRequestDto(
                    ShippingAddressId: shippingAddressId,
                    BillingAddressId: billingAddressId,
                    PaymentMethod: Application.Enums.PaymentMethod.CreditCard,
                    ShippingCost: 0m,
                    TaxAmount: 0m,
                    Notes: null,
                    OrderDetails: orderDetails
                );

                _logger.LogInformation("Calling OrderService.CreateOrderAsync...");
                var orderResult = await _orderService.CreateOrderAsync(createOrderRequest);
                
                _logger.LogInformation("Order creation result. IsSuccess: {IsSuccess}, OrderId: {OrderId}", 
                    orderResult?.IsSuccess, orderResult?.Data?.Id);
                
                if (orderResult?.IsSuccess != true || orderResult.Data == null)
                {
                    _logger.LogWarning("Order creation failed. Error: {Error}", orderResult?.ErrorMessage);
                    TempData["ErrorMessage"] = orderResult?.ErrorMessage ?? "Sipariş oluşturulamadı.";
                    return RedirectToAction("Index");
                }

                var orderId = orderResult.Data.Id;
                _logger.LogInformation("Order created successfully. OrderId: {OrderId}", orderId);
                
                // Sipariş başarıyla oluşturuldu, sepeti temizle
                try
                {
                    await _cartService.ClearCartAsync();
                    _logger.LogInformation("Cart cleared after order creation. OrderId: {OrderId}", orderId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to clear cart after order creation. OrderId: {OrderId}", orderId);
                    // Sepet temizlenemese bile sipariş oluşturuldu, devam et
                }
                
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                _logger.LogInformation("User email: {Email}", userEmail);

                // Stripe Checkout Session oluştur
                // Total'ı cent'e çevir (örnek: $16.64 = 1664 cent)
                var amountInCents = (long)(total * 100);
                
                _logger.LogInformation("Creating Stripe checkout session. OrderId: {OrderId}, Amount: {Amount} cents", 
                    orderId, amountInCents);
                
                string checkoutUrl;
                try
                {
                    checkoutUrl = await _paymentService.CreateCheckoutSessionAsync(
                        amount: amountInCents,
                        currency: "usd",
                        orderId: orderId.ToString(),
                        customerEmail: userEmail
                    );
                    
                    _logger.LogInformation("Stripe checkout session created. URL: {Url}", checkoutUrl);
                }
                catch (StripeException stripeEx)
                {
                    _logger.LogError(stripeEx, "Stripe API error. Code: {Code}, Message: {Message}", 
                        stripeEx.StripeError?.Code, stripeEx.Message);
                    TempData["ErrorMessage"] = $"Stripe hatası: {stripeEx.Message}";
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrEmpty(checkoutUrl))
                {
                    _logger.LogError("Stripe checkout URL is null or empty");
                    TempData["ErrorMessage"] = "Stripe ödeme sayfası oluşturulamadı.";
                    return RedirectToAction("Index");
                }

                // Kullanıcıyı Stripe Checkout sayfasına yönlendir
                return Redirect(checkoutUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment creation failed");
                TempData["ErrorMessage"] = "Ödeme işlemi başlatılamadı. Lütfen tekrar deneyin.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Stripe ödeme başarılı olduğunda buraya yönlendirilir
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Stripe'dan döndüğünde session kaybolabilir, bu yüzden AllowAnonymous
        public async Task<IActionResult> Success(string session_id)
        {
            try
            {
                if (string.IsNullOrEmpty(session_id))
                {
                    TempData["ErrorMessage"] = "Geçersiz ödeme oturumu.";
                    return RedirectToAction("Index");
                }

                // Stripe'dan session bilgilerini al
                var session = await _paymentService.GetCheckoutSessionAsync(session_id);

                if (session.PaymentStatus == "paid")
                {
                    // Metadata'dan orderId'yi al
                    if (session.Metadata.TryGetValue("orderId", out var orderIdStr) && 
                        Guid.TryParse(orderIdStr, out var orderId))
                    {
                        // Sipariş ödeme durumunu Paid olarak güncelle
                        try
                        {
                            _logger.LogInformation("Attempting to update payment status. OrderId: {OrderId}, PaymentStatus: Paid", orderId);
                            
                            var updateResult = await _orderService.UpdatePaymentStatusAsync(
                                orderId, 
                                Application.Enums.PaymentStatus.Paid
                            );
                            
                            if (updateResult?.IsSuccess == true)
                            {
                                _logger.LogInformation("✅ Order payment status updated to Paid successfully. OrderId: {OrderId}", orderId);
                            }
                            else
                            {
                                _logger.LogError("❌ Failed to update order payment status. OrderId: {OrderId}, Error: {Error}", 
                                    orderId, updateResult?.ErrorMessage ?? "Unknown error");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ Exception updating order payment status. OrderId: {OrderId}, Exception: {Exception}", 
                                orderId, ex.Message);
                            // Ödeme başarılı ama durum güncellenemedi, devam et
                        }
                        
                        // Sepet zaten CreatePayment içinde temizlendi, burada tekrar temizlemeye gerek yok
                        TempData["SuccessMessage"] = "Ödemeniz başarıyla tamamlandı! Siparişiniz hazırlanıyor.";
                        _logger.LogInformation("Payment successful. OrderId: {OrderId}, SessionId: {SessionId}", 
                            orderId, session_id);
                        
                        // Her zaman sepet sayfasına yönlendir (sepet zaten boş olacak çünkü sipariş oluşturuldu)
                        return RedirectToAction("Index", "Cart");
                    }
                }

                TempData["ErrorMessage"] = "Ödeme işlemi tamamlanamadı.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment success handling failed. SessionId: {SessionId}", session_id);
                TempData["ErrorMessage"] = "Ödeme işlemi kontrol edilemedi.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Kullanıcı ödemeyi iptal ederse buraya yönlendirilir
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Stripe'dan döndüğünde session kaybolabilir, bu yüzden AllowAnonymous
        public IActionResult Cancel()
        {
            TempData["InfoMessage"] = "Ödeme işlemi iptal edildi. Siparişiniz kaydedilmedi.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Siparişi tamamlar (Eski metod - artık kullanılmıyor, Stripe entegrasyonu için CreatePayment kullanılıyor)
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(
            Guid shippingAddressId,
            Guid? billingAddressId)
        {
            // Sepeti kontrol et
            var cartResult = await _cartService.GetMyCartAsync();
            var cartItems = cartResult?.IsSuccess == true && cartResult.Data != null
                ? cartResult.Data
                : new List<CartItemDto>();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Sepetiniz boş.";
                return RedirectToAction("Index", "Cart");
            }

            // Validasyon
            if (shippingAddressId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Lütfen teslimat adresi seçin.";
                return RedirectToAction("Index");
            }

            // OrderDetails oluştur
            var orderDetails = cartItems.Select(item => new OrderItemRequestDto(
                item.ProductId,
                item.Quantity
            )).ToList();

            // Sipariş oluştur (PaymentMethod default olarak CreditCard - Stripe entegrasyonu için)
            var total = cartItems.Sum(item => item.SubTotal);
            var createOrderRequest = new CreateOrderRequestDto(
                ShippingAddressId: shippingAddressId,
                BillingAddressId: billingAddressId,
                PaymentMethod: Application.Enums.PaymentMethod.CreditCard, // Stripe için default
                ShippingCost: 0m, // Ücretsiz kargo
                TaxAmount: 0m, // Vergi hesaplaması yok
                Notes: null, // Order notes kaldırıldı
                OrderDetails: orderDetails
            );

            var orderResult = await _orderService.CreateOrderAsync(createOrderRequest);

            if (orderResult?.IsSuccess == true && orderResult.Data != null)
            {
                // Sepeti temizle
                await _cartService.ClearCartAsync();

                TempData["SuccessMessage"] = "Siparişiniz başarıyla oluşturuldu!";
                _logger.LogInformation("Order created successfully. OrderId: {OrderId}", orderResult.Data.Id);
                
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = orderResult?.ErrorMessage ?? "Sipariş oluşturulamadı. Lütfen tekrar deneyin.";
            _logger.LogWarning("Order creation failed. Error: {Error}", orderResult?.ErrorMessage);
            return RedirectToAction("Index");
        }
    }
}
