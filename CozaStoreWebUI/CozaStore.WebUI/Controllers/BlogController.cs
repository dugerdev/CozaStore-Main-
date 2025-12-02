using CozaStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly BlogService _blogService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(BlogService blogService, ILogger<BlogController> logger)
        {
            _blogService = blogService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Blog";
            ViewData["ActiveMenu"] = "Blog";
            ViewData["MenuShadow"] = "yes";

            var result = await _blogService.GetPublishedAsync();
            
            if (!result.IsSuccess)
            {
                _logger.LogWarning("Blog gönderileri yüklenemedi: {Error}", result.ErrorMessage);
                return View(new List<CozaStore.Application.DTOs.BlogPostDto>());
            }

            return View(result.Data ?? new List<CozaStore.Application.DTOs.BlogPostDto>());
        }

        public async Task<IActionResult> Detail(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ViewData["Title"] = "Blog Detay";
            ViewData["ActiveMenu"] = "Blog";
            ViewData["MenuShadow"] = "yes";

            var result = await _blogService.GetByIdAsync(id.Value);
            
            if (!result.IsSuccess || result.Data == null)
            {
                _logger.LogWarning("Blog gönderisi bulunamadı: {Id}, Error: {Error}", id, result.ErrorMessage);
                TempData["ErrorMessage"] = "Blog gönderisi bulunamadı.";
                return RedirectToAction("Index");
            }

            return View(result.Data);
        }
    }
}
