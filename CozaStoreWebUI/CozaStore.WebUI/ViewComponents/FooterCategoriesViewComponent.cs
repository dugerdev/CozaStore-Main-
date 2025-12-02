using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.ViewComponents
{
    public class FooterCategoriesViewComponent : ViewComponent
    {
        private readonly CategoryService _categoryService;

        public FooterCategoriesViewComponent(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoryResult = await _categoryService.GetAllAsync();
            var categories = categoryResult?.IsSuccess == true 
                ? categoryResult.Data ?? new List<CategoryDto>()
                : new List<CategoryDto>();

            return View(categories);
        }
    }
}


