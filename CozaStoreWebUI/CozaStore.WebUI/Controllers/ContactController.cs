using CozaStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactService _contactService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(ContactService contactService, ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Contact";
            ViewData["ActiveMenu"] = "Contact";
            ViewData["MenuShadow"] = "yes";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string email, string msg)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(msg))
            {
                TempData["ErrorMessage"] = "Email ve mesaj alanları zorunludur.";
                return RedirectToAction("Index");
            }

            try
            {
                var result = await _contactService.SendMessageAsync(email, msg);

                if (result?.IsSuccess == true)
                {
                    _logger.LogInformation("Contact form başarıyla gönderildi. Email: {Email}", email);
                    TempData["SuccessMessage"] = result.Message ?? "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.";
                }
                else
                {
                    _logger.LogWarning("Contact form gönderilemedi. Email: {Email}, Error: {Error}", email, result?.Message);
                    TempData["ErrorMessage"] = result?.Message ?? "Mesaj gönderilemedi. Lütfen tekrar deneyin.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contact form gönderilirken hata oluştu. Email: {Email}", email);
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
            }

            return RedirectToAction("Index");
        }
    }
}

