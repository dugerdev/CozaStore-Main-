using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CozaStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        private readonly CartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(CartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        /// <summary>
        /// Sepet sayfasını gösterir
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var cartResult = await _cartService.GetMyCartAsync();
            
            var cartItems = cartResult?.IsSuccess == true 
                ? cartResult.Data ?? new List<CartItemDto>()
                : new List<CartItemDto>();

            var total = cartItems.Sum(item => item.SubTotal);

            ViewData["CartItems"] = cartItems;
            ViewData["Total"] = total;
            ViewData["Title"] = "Shopping Cart";

            return View();
        }

        /// <summary>
        /// Sepete ürün ekler
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add(Guid productId, int quantity = 1)
        {
            _logger.LogInformation("Add to cart called. ProductId: {ProductId}, Quantity: {Quantity}, User: {UserId}", 
                productId, quantity, User.Identity?.Name);
            
            // Token kontrolü
            var token = Request.Cookies["AuthToken"];
            _logger.LogInformation("AuthToken cookie present: {HasToken}, Token length: {TokenLength}", 
                !string.IsNullOrEmpty(token), token?.Length ?? 0);
            
            if (productId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Geçersiz ürün.";
                return RedirectToAction("Index", "Shop");
            }

            if (quantity <= 0)
            {
                quantity = 1;
            }

            var request = new AddToCartRequestDto(productId, quantity);
            var result = await _cartService.AddToCartAsync(request);

            if (result?.IsSuccess == true)
            {
                _logger.LogInformation("Product added to cart successfully. ProductId: {ProductId}", productId);
                TempData["SuccessMessage"] = "Ürün sepete eklendi.";
                
                // Wishlist'ten kaldır (eğer wishlist'te varsa)
                bool wasInWishlist = false;
                try
                {
                    var wishlistJson = Request.Cookies["Wishlist"];
                    if (!string.IsNullOrEmpty(wishlistJson))
                    {
                        var wishlist = JsonSerializer.Deserialize<List<Guid>>(wishlistJson) ?? new List<Guid>();
                        if (wishlist.Contains(productId))
                        {
                            wishlist.Remove(productId);
                            var updatedJson = JsonSerializer.Serialize(wishlist);
                            var cookieOptions = new CookieOptions
                            {
                                Expires = DateTimeOffset.UtcNow.AddDays(7),
                                HttpOnly = true,
                                Secure = Request.IsHttps,
                                SameSite = SameSiteMode.Lax,
                                Path = "/"
                            };
                            Response.Cookies.Append("Wishlist", updatedJson, cookieOptions);
                            wasInWishlist = true;
                            _logger.LogInformation("Product removed from wishlist. ProductId: {ProductId}", productId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Wishlist'ten ürün kaldırılırken hata oluştu. ProductId: {ProductId}", productId);
                }
                
                // AJAX isteği ise JSON döndür
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    var cartResult = await _cartService.GetMyCartAsync();
                    var count = cartResult?.IsSuccess == true && cartResult.Data != null
                        ? cartResult.Data.Sum(item => item.Quantity)
                        : 0;
                    return Json(new { success = true, count, removedFromWishlist = wasInWishlist });
                }
                
                // Eğer wishlist sayfasından geliyorsa, wishlist sayfasına yönlendir
                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer) && referer.Contains("/Wishlist"))
                {
                    return RedirectToAction("Index", "Wishlist");
                }
            }
            else
            {
                _logger.LogWarning("Failed to add product to cart. ProductId: {ProductId}, Error: {Error}", 
                    productId, result?.ErrorMessage);
                TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürün sepete eklenemedi.";
                
                // AJAX isteği ise JSON döndür
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, error = result?.ErrorMessage ?? "Ürün sepete eklenemedi." });
                }
            }

            return RedirectToAction("Index", "Shop");
        }

        /// <summary>
        /// Sepetteki ürün miktarını günceller
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(Guid cartItemId, int quantity)
        {
            if (cartItemId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Geçersiz sepet öğesi.";
                return RedirectToAction("Index");
            }

            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Miktar 0'dan büyük olmalıdır.";
                return RedirectToAction("Index");
            }

            var result = await _cartService.UpdateQuantityAsync(cartItemId, quantity);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Miktar güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.ErrorMessage ?? "Miktar güncellenemedi.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Sepetten ürün siler
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Remove(Guid cartItemId)
        {
            if (cartItemId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Geçersiz sepet öğesi.";
                return RedirectToAction("Index");
            }

            var result = await _cartService.RemoveFromCartAsync(cartItemId);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Ürün sepetten çıkarıldı.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürün silinemedi.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Sepeti tamamen temizler
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            var result = await _cartService.ClearCartAsync();

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Sepet temizlendi.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.ErrorMessage ?? "Sepet temizlenemedi.";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Sepetteki ürün sayısını döndürür (AJAX için)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            try
            {
                var cartResult = await _cartService.GetMyCartAsync();
                var count = cartResult?.IsSuccess == true && cartResult.Data != null
                    ? cartResult.Data.Sum(item => item.Quantity)
                    : 0;

                return Json(new { count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        /// <summary>
        /// Sepet paneli için veri döndürür (AJAX için)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPanelData()
        {
            try
            {
                var cartResult = await _cartService.GetMyCartAsync();
                var cartItems = cartResult?.IsSuccess == true && cartResult.Data != null
                    ? cartResult.Data
                    : new List<CartItemDto>();
                
                var total = cartItems.Sum(item => item.SubTotal);

                return Json(new 
                { 
                    items = cartItems.Select(item => new
                    {
                        productId = item.ProductId,
                        productName = item.ProductName,
                        productImageUrl = item.ProductImageUrl ?? "/images/product-01.jpg",
                        quantity = item.Quantity,
                        productPrice = item.ProductPrice,
                        subTotal = item.SubTotal
                    }),
                    total = total
                });
            }
            catch
            {
                return Json(new { items = new List<object>(), total = 0m });
            }
        }
    }
}


