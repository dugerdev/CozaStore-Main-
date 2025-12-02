using System.Diagnostics;
using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using CozaStore.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;
        private readonly CategoryService _categoryService;

        public HomeController(CategoryService categoryService, ProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var productsResult = await _productService.GetAllAsync();
            var categoriesResult = await _categoryService.GetAllAsync();

            ViewData["Products"] = productsResult?.IsSuccess == true ? productsResult.Data : new List<ProductDto>();
            ViewData["Categories"] = categoriesResult?.IsSuccess == true ? categoriesResult.Data : new List<CategoryDto>();
            ViewData["Title"] = "Home";

            return View();
        }

    }
}
