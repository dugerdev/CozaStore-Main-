using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using CozaStore.WebUI.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.ViewComponents
{
    public class CartPanelViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartPanelViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartItems = new List<CartItemDto>();
            decimal total = 0;

            if (User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var cartResult = await _cartService.GetMyCartAsync();
                    if (cartResult?.IsSuccess == true && cartResult.Data != null)
                    {
                        cartItems = cartResult.Data;
                        total = cartItems.Sum(item => item.SubTotal);
                    }
                }
                catch
                {
                    // Hata durumunda boş liste döndür
                }
            }

            var viewModel = new CartPanelViewModel
            {
                Items = cartItems,
                Total = total
            };

            return View(viewModel);
        }
    }
}

