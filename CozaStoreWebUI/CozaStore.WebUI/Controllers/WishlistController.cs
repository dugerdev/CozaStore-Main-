using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CozaStore.WebUI.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ProductService _productService;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(ProductService productService, ILogger<WishlistController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Wishlist";
            ViewData["ActiveMenu"] = "Wishlist";
            ViewData["MenuShadow"] = "yes";
            ViewData["HeaderClass"] = "header-v4";
            
            // Cookie'den wishlist'i al (Guid listesi) - Session yerine Cookie kullan
            var wishlistJson = Request.Cookies["Wishlist"];
            var productIds = new List<Guid>();
            
            if (!string.IsNullOrEmpty(wishlistJson))
            {
                try
                {
                    productIds = JsonSerializer.Deserialize<List<Guid>>(wishlistJson) ?? new List<Guid>();
                }
                catch
                {
                    productIds = new List<Guid>();
                }
            }
            
            // Her bir ürün ID'si için ürün bilgilerini çek
            var wishlistProducts = new List<ProductDto>();
            foreach (var productId in productIds)
            {
                try
                {
                    var productResult = await _productService.GetByIdAsync(productId);
                    if (productResult?.IsSuccess == true && productResult.Data != null)
                    {
                        wishlistProducts.Add(productResult.Data);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Ürün bilgisi alınamadı. ProductId: {ProductId}, Error: {Error}", productId, ex.Message);
                }
            }
            
            ViewData["WishlistItems"] = wishlistProducts;
            
            return View();
        }

        /// <summary>
        /// Wishlist'teki ürün sayısını döndürür (AJAX için)
        /// </summary>
        [HttpGet]
        public IActionResult GetCount()
        {
            try
            {
                var wishlist = Request.Cookies["Wishlist"];
                int count = 0;
                
                if (!string.IsNullOrEmpty(wishlist))
                {
                    try
                    {
                        var wishlistItems = JsonSerializer.Deserialize<List<Guid>>(wishlist);
                        count = wishlistItems?.Count ?? 0;
                    }
                    catch
                    {
                        count = 0;
                    }
                }

                return Json(new { count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        /// <summary>
        /// Wishlist'e ürün ekler
        /// </summary>
        [HttpPost]
        public IActionResult Add(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return Json(new { success = false, error = "Geçersiz ürün ID'si." });
            }

            try
            {
                // Cookie'den mevcut wishlist'i al
                var wishlistJson = Request.Cookies["Wishlist"];
                var wishlist = new List<Guid>();

                if (!string.IsNullOrEmpty(wishlistJson))
                {
                    try
                    {
                        wishlist = JsonSerializer.Deserialize<List<Guid>>(wishlistJson) ?? new List<Guid>();
                    }
                    catch
                    {
                        wishlist = new List<Guid>();
                    }
                }

                // Ürün zaten wishlist'te değilse ekle
                if (!wishlist.Contains(productId))
                {
                    wishlist.Add(productId);
                    
                    // Cookie'ye kaydet (7 gün geçerli)
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
                    
                    return Json(new { success = true, count = wishlist.Count });
                }
                else
                {
                    return Json(new { success = false, error = "Ürün zaten wishlist'te." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wishlist'e ürün eklenirken hata oluştu. ProductId: {ProductId}", productId);
                return Json(new { success = false, error = "Wishlist'e eklenirken hata oluştu." });
            }
        }

        /// <summary>
        /// Wishlist'ten ürün çıkarır
        /// </summary>
        [HttpPost]
        public IActionResult Remove(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return Json(new { success = false, error = "Geçersiz ürün ID'si." });
            }

            try
            {
                // Cookie'den mevcut wishlist'i al
                var wishlistJson = Request.Cookies["Wishlist"];
                var wishlist = new List<Guid>();

                if (!string.IsNullOrEmpty(wishlistJson))
                {
                    try
                    {
                        wishlist = JsonSerializer.Deserialize<List<Guid>>(wishlistJson) ?? new List<Guid>();
                    }
                    catch
                    {
                        wishlist = new List<Guid>();
                    }
                }

                // Ürünü wishlist'ten çıkar
                wishlist.Remove(productId);
                
                // Cookie'ye kaydet (7 gün geçerli)
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
                
                TempData["SuccessMessage"] = "Ürün wishlist'ten çıkarıldı.";
                
                // AJAX isteği ise JSON döndür
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, count = wishlist.Count });
                }
                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wishlist'ten ürün çıkarılırken hata oluştu. ProductId: {ProductId}", productId);
                TempData["ErrorMessage"] = "Wishlist'ten çıkarılırken hata oluştu.";
                
                // AJAX isteği ise JSON döndür
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, error = "Wishlist'ten çıkarılırken hata oluştu." });
                }
                
                return RedirectToAction("Index");
            }
        }
    }
}

