using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BlogPostsController : Controller
{
    private readonly AdminBlogPostService _blogPostService;
    private readonly ILogger<BlogPostsController> _logger;

    public BlogPostsController(
        AdminBlogPostService blogPostService,
        ILogger<BlogPostsController> logger)
    {
        _blogPostService = blogPostService;
        _logger = logger;
    }

    // GET: Admin/BlogPosts
    public async Task<IActionResult> Index()
    {
        var result = await _blogPostService.GetAllAsync();
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Blog gönderileri yüklenemedi.";
            return View(new List<BlogPostDto>());
        }

        return View(result.Data ?? new List<BlogPostDto>());
    }

    // GET: Admin/BlogPosts/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _blogPostService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Blog gönderisi bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // GET: Admin/BlogPosts/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/BlogPosts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBlogPostRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _blogPostService.CreateAsync(request);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Blog gönderisi oluşturulamadı.";
            return View(request);
        }

        TempData["SuccessMessage"] = "Blog gönderisi başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/BlogPosts/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _blogPostService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Blog gönderisi bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var blogPost = result.Data;
        var updateRequest = new UpdateBlogPostRequestDto(
            blogPost.Id,
            blogPost.Title,
            blogPost.Content,
            blogPost.ImageUrl,
            blogPost.IsPublished
        );

        return View(updateRequest);
    }

    // POST: Admin/BlogPosts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateBlogPostRequestDto request)
    {
        if (id != request.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(request);
        }

        var result = await _blogPostService.UpdateAsync(id, request);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Blog gönderisi güncellenemedi.";
            return View(request);
        }

        TempData["SuccessMessage"] = "Blog gönderisi başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // GET: Admin/BlogPosts/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _blogPostService.GetByIdAsync(id);
        
        if (result?.IsSuccess != true || result.Data == null)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Blog gönderisi bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        return View(result.Data);
    }

    // POST: Admin/BlogPosts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _blogPostService.DeleteAsync(id);
        
        if (result?.IsSuccess != true)
        {
            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Blog gönderisi silinemedi.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Blog gönderisi başarıyla silindi.";
        return RedirectToAction(nameof(Index));
    }
}
