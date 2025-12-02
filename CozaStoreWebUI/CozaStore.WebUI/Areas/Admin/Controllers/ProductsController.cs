using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AdminProductService _productService;
    private readonly CategoryService _categoryService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        AdminProductService productService,
        CategoryService categoryService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _categoryService = categoryService;
        _logger = logger;
    }

    // GET: Admin/Products
    public async Task<IActionResult> Index()
    {
        var result = await _productService.GetAllAsync();
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürünler yüklenemedi.";
            return View(new List<ProductDto>());
        }

        return View(result.Data ?? new List<ProductDto>());
    }

    // GET: Admin/Products/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _productService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // GET: Admin/Products/Create
    public async Task<IActionResult> Create()
    {
        // Kategorileri yükle
        var categoriesResult = await _categoryService.GetAllAsync();
        ViewBag.Categories = categoriesResult?.Data ?? new List<CategoryDto>();
        
        return View();
    }

    // POST: Admin/Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var categoriesResult = await _categoryService.GetAllAsync();
            ViewBag.Categories = categoriesResult?.Data ?? new List<CategoryDto>();
            return View(request);
        }

        var result = await _productService.CreateAsync(request);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürün oluşturulamadı.";
            var categoriesResult = await _categoryService.GetAllAsync();
            ViewBag.Categories = categoriesResult?.Data ?? new List<CategoryDto>();
            return View(request);
        }

        TempData["SuccessMessage"] = "Ürün başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Products/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _productService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürün bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        // Kategorileri yükle
        var categoriesResult = await _categoryService.GetAllAsync();
        ViewBag.Categories = categoriesResult?.Data ?? new List<CategoryDto>();

        var product = result.Data;
        var updateRequest = new UpdateProductRequestDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.ImageUrl,
            product.CategoryId,
            product.IsActive
        );

        return View(updateRequest);
    }

    // POST: Admin/Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProductRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var categoriesResult = await _categoryService.GetAllAsync();
            ViewBag.Categories = categoriesResult?.Data ?? new List<CategoryDto>();
            return View(request);
        }

        var result = await _productService.UpdateAsync(request);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürün güncellenemedi.";
            var categoriesResult = await _categoryService.GetAllAsync();
            ViewBag.Categories = categoriesResult?.Data ?? new List<CategoryDto>();
            return View(request);
        }

        TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Products/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.DeleteAsync(id);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Ürün silinemedi.";
        }
        else
        {
            TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
        }

        return RedirectToAction(nameof(Index));
    }
}

