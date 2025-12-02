using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.ViewComponents
{
    public class WishlistCountViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            // Cookie'den wishlist sayısını al
            int count = 0;
            
            if (User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var wishlistJson = HttpContext.Request.Cookies["Wishlist"];
                    if (!string.IsNullOrEmpty(wishlistJson))
                    {
                        var wishlist = System.Text.Json.JsonSerializer.Deserialize<List<System.Guid>>(wishlistJson);
                        count = wishlist?.Count ?? 0;
                    }
                }
                catch
                {
                    count = 0;
                }
            }

            return Task.FromResult<IViewComponentResult>(View(count));
        }
    }
}

