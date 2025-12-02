using CozaStore.Application.DTOs;

namespace CozaStore.WebUI.ViewModels
{
    public class CartPanelViewModel
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal Total { get; set; }
    }
}


