using CozaStore.Application.Common;
using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;
        private readonly ILogger<ShopController> _logger;

        public ShopController(CategoryService categoryService, ProductService productService, ILogger<ShopController> logger)
        {
            _categoryService = categoryService;
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(Guid? categoryId, string? search, string? sortBy, decimal? minPrice, decimal? maxPrice, string? tag)
        {
            _logger.LogInformation("Shop/Index çağrıldı. CategoryId: {CategoryId}, Search: {Search}, SortBy: {SortBy}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Tag: {Tag}", 
                categoryId, search, sortBy, minPrice, maxPrice, tag);

            // Kategorileri çek
            var categoryResult = await _categoryService.GetAllAsync();
            var categories = categoryResult?.IsSuccess == true 
                ? categoryResult.Data ?? new List<CategoryDto>()
                : new List<CategoryDto>();

            _logger.LogInformation("Kategoriler yüklendi. Count: {Count}, IsSuccess: {IsSuccess}, Error: {Error}", 
                categories.Count, categoryResult?.IsSuccess, categoryResult?.ErrorMessage);

            // Ürünleri çek
            Result<List<ProductDto>>? productResult;
            List<ProductDto> products;
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                // Arama yapılıyorsa tüm ürünleri getir ve filtrele
                productResult = await _productService.GetAllAsync();
                if (productResult?.IsSuccess == true && productResult.Data != null)
                {
                    var searchLower = search.ToLower();
                    products = productResult.Data
                        .Where(p => p.Name.ToLower().Contains(searchLower) || 
                                   (p.Description != null && p.Description.ToLower().Contains(searchLower)))
                        .ToList();
                }
                else
                {
                    products = new List<ProductDto>();
                }
            }
            else if (categoryId.HasValue)
            {
                productResult = await _productService.GetByCategoryAsync(categoryId.Value);
                products = productResult?.IsSuccess == true 
                    ? productResult.Data ?? new List<ProductDto>()
                    : new List<ProductDto>();
            }
            else
            {
                productResult = await _productService.GetAllAsync();
                products = productResult?.IsSuccess == true 
                    ? productResult.Data ?? new List<ProductDto>()
                    : new List<ProductDto>();
            }

            // Fiyat filtreleme
            if (minPrice.HasValue || maxPrice.HasValue)
            {
                products = products.Where(p => 
                    (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || p.Price <= maxPrice.Value)
                ).ToList();
            }

            // Tag filtreleme - Her tag için farklı rastgele ürünler göster
            if (!string.IsNullOrWhiteSpace(tag))
            {
                // Her tag için farklı bir sıralama yaparak rastgele görünüm sağla
                // Tag hash'ini seed olarak kullanarak deterministik rastgelelik sağla
                var tagHash = Math.Abs(tag.GetHashCode());
                
                // Her tag için farklı sıralama kriteri kullan
                // Tag hash'ine göre farklı sıralama yöntemleri
                switch (tagHash % 4)
                {
                    case 0:
                        // ID'ye göre ters sıralama
                        products = products.OrderByDescending(p => p.Id).ToList();
                        break;
                    case 1:
                        // Name'e göre sıralama
                        products = products.OrderBy(p => p.Name).ToList();
                        break;
                    case 2:
                        // Price'a göre ters sıralama
                        products = products.OrderByDescending(p => p.Price).ToList();
                        break;
                    case 3:
                        // CreatedDate'e göre sıralama
                        products = products.OrderBy(p => p.CreatedDate).ToList();
                        break;
                    default:
                        // ID'ye göre sıralama
                        products = products.OrderBy(p => p.Id).ToList();
                        break;
                }
            }

            // Sıralama
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price-low":
                        products = products.OrderBy(p => p.Price).ToList();
                        break;
                    case "price-high":
                        products = products.OrderByDescending(p => p.Price).ToList();
                        break;
                    case "newness":
                        // Newness: En yeni ürünler önce (CreatedDate DESC)
                        products = products.OrderByDescending(p => p.CreatedDate).ToList();
                        break;
                    case "popularity":
                        // Popularity: Rastgele farklı bir sıralama (Name'e göre)
                        products = products.OrderBy(p => p.Name).ToList();
                        break;
                    case "rating":
                        // Rating: Rastgele farklı bir sıralama (Id'ye göre)
                        products = products.OrderBy(p => p.Id).ToList();
                        break;
                    default:
                        // Default: Rastgele farklı bir sıralama (Price'a göre ters)
                        products = products.OrderByDescending(p => p.Price).ThenBy(p => p.Name).ToList();
                        break;
                }
            }
            else
            {
                // Default sıralama: CreatedDate'e göre
                products = products.OrderByDescending(p => p.CreatedDate).ToList();
            }

            _logger.LogInformation("Ürünler yüklendi. Count: {Count}, IsSuccess: {IsSuccess}, Error: {Error}", 
                products.Count, productResult?.IsSuccess, productResult?.ErrorMessage);

            ViewData["Products"] = products;
            ViewData["Categories"] = categories;
            ViewData["SelectedCategoryId"] = categoryId;
            ViewData["Search"] = search;
            ViewData["SortBy"] = sortBy;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["Tag"] = tag;
            ViewData["Title"] = "Shop";

            return View();
        }

        public async Task<IActionResult> Product(Guid id)
        {
            var productResult = await _productService.GetByIdAsync(id);

            if(productResult?.IsSuccess != true || productResult.Data == null)
            {
                return RedirectToAction("Index");
            }

            ViewData["Product"] = productResult.Data;
            ViewData["Title"] = productResult.Data.Name;

            return View();
        }
    }
}
