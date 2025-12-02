using CozaStore.Application.DTOs;
using CozaStore.Application.Enums;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController : Controller
{
    private readonly AdminOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        AdminOrderService orderService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    // GET: Admin/Orders
    public async Task<IActionResult> Index()
    {
        var result = await _orderService.GetAllAsync();
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Siparişler yüklenemedi.";
            return View(new List<OrderDto>());
        }

        return View(result.Data ?? new List<OrderDto>());
    }

    // GET: Admin/Orders/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _orderService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Sipariş bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // POST: Admin/Orders/UpdateStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, OrderStatus status)
    {
        var result = await _orderService.UpdateStatusAsync(id, status);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Sipariş durumu güncellenemedi.";
        }
        else
        {
            TempData["SuccessMessage"] = "Sipariş durumu başarıyla güncellendi.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}

