using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReviewsController : Controller
{
    private readonly AdminReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(
        AdminReviewService reviewService,
        ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    // GET: Admin/Reviews
    public async Task<IActionResult> Index()
    {
        var result = await _reviewService.GetAllAsync();
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Yorumlar yüklenemedi.";
            return View(new List<ReviewDto>());
        }

        return View(result.Data ?? new List<ReviewDto>());
    }

    // POST: Admin/Reviews/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _reviewService.ApproveAsync(id);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Yorum onaylanamadı.";
        }
        else
        {
            TempData["SuccessMessage"] = "Yorum başarıyla onaylandı.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Reviews/Reject/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await _reviewService.RejectAsync(id);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Yorum reddedilemedi.";
        }
        else
        {
            TempData["SuccessMessage"] = "Yorum başarıyla reddedildi.";
        }

        return RedirectToAction(nameof(Index));
    }
}

