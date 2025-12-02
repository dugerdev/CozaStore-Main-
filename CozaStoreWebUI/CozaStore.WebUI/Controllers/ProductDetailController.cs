using CozaStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Controllers;


public class ProductDetailController : Controller
{
    private readonly ProductService _productService;
    private readonly ILogger<ProductDetailController> _logger;

    public ProductDetailController(ProductService productService, ILogger<ProductDetailController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("ProductDetail/Index çağırıldı ancak geçersiz ID: {Id}", id);
            TempData["ErrorMessage"] = "Geçersiz ürün ID'si.";
            return RedirectToAction("Index", "Shop");
        }

        _logger.LogInformation("ProductDetail/Index çağırıldı. ProductId: {ProductId}", id);

        var productResult = await _productService.GetByIdAsync(id);

        if (productResult?.IsSuccess != true || productResult.Data == null)
        {
            _logger.LogWarning("Ürün bulunamadı. ProductId: {ProductId}, IsSuccess: {IsSuccess}, Error: {Error}",
                id, productResult?.IsSuccess, productResult?.ErrorMessage);
            TempData["ErrorMessage"] = productResult?.ErrorMessage ?? "Ürün Bulunmadı.";
            return RedirectToAction("Index", "Shop");
        }

        var product = productResult.Data;

        ViewData["Product"] = product;
        ViewData["Title"] = product.Name;
        ViewData["ActiveMenu"] = "Shop";
        ViewData["HeaderClass"] = "header-v4";
        ViewData["MenuShadow"] = "yes";

        _logger.LogInformation("Ürün detayı yüklendiç ProductId: {ProductId}, Name: {Name}",
            product.Id, product.Name);

        return View();
    }
}
