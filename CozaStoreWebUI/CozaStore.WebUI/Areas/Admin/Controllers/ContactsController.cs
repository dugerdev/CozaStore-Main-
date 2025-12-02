using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ContactsController : Controller
{
    private readonly AdminContactService _contactService;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(
        AdminContactService contactService,
        ILogger<ContactsController> logger)
    {
        _contactService = contactService;
        _logger = logger;
    }

    // GET: Admin/Contacts
    public async Task<IActionResult> Index()
    {
        var result = await _contactService.GetAllAsync();
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Mesajlar yüklenemedi.";
            return View(new List<ContactDto>());
        }

        return View(result.Data ?? new List<ContactDto>());
    }

    // GET: Admin/Contacts/Unread
    public async Task<IActionResult> Unread()
    {
        var result = await _contactService.GetUnreadAsync();
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Okunmamış mesajlar yüklenemedi.";
            return View(new List<ContactDto>());
        }

        return View(result.Data ?? new List<ContactDto>());
    }

    // GET: Admin/Contacts/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _contactService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Mesaj bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        // Mesajı okundu olarak işaretle
        await _contactService.MarkAsReadAsync(id);

        return View(result.Data);
    }

    // POST: Admin/Contacts/MarkAsRead/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _contactService.MarkAsReadAsync(id);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Mesaj okundu olarak işaretlenemedi.";
        }
        else
        {
            TempData["SuccessMessage"] = "Mesaj okundu olarak işaretlendi.";
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: Admin/Contacts/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _contactService.DeleteAsync(id);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Mesaj silinemedi.";
        }
        else
        {
            TempData["SuccessMessage"] = "Mesaj başarıyla silindi.";
        }

        return RedirectToAction(nameof(Index));
    }
}

