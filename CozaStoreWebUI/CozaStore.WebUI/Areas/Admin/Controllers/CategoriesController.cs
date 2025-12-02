using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly AdminCategoryService _categoryService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        AdminCategoryService categoryService,
        ILogger<CategoriesController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    // GET: Admin/Categories
    public async Task<IActionResult> Index()
    {
        var result = await _categoryService.GetAllAsync();
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Kategoriler yüklenemedi.";
            return View(new List<CategoryDto>());
        }

        return View(result.Data ?? new List<CategoryDto>());
    }

    // GET: Admin/Categories/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // GET: Admin/Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _categoryService.CreateAsync(request);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Kategori oluşturulamadı.";
            return View(request);
        }

        TempData["SuccessMessage"] = "Kategori başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/Categories/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Kategori bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var category = result.Data;
        var updateRequest = new UpdateCategoryRequestDto(
            category.Id,
            category.Name,
            category.Description,
            category.ImageUrl,
            category.IsActive
        );

        return View(updateRequest);
    }

    // POST: Admin/Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateCategoryRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _categoryService.UpdateAsync(request);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Kategori güncellenemedi.";
            return View(request);
        }

        TempData["SuccessMessage"] = "Kategori başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Categories/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Kategori silinemedi.";
        }
        else
        {
            TempData["SuccessMessage"] = "Kategori başarıyla silindi.";
        }

        return RedirectToAction(nameof(Index));
    }
}

