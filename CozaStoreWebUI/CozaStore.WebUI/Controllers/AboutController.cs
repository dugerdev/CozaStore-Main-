using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "About";
            ViewData["ActiveMenu"] = "About";
            ViewData["MenuShadow"] = "yes";
            return View();
        }
    }
}


