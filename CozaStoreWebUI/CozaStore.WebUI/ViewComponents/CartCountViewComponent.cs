using CozaStore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartCountViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = 0;
            
            if (User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var cartResult = await _cartService.GetMyCartAsync();
                    if (cartResult?.IsSuccess == true && cartResult.Data != null)
                    {
                        count = cartResult.Data.Sum(item => item.Quantity);
                    }
                }
                catch
                {
                    // Hata durumunda 0 döndür
                }
            }

            return View(count);
        }
    }
}


