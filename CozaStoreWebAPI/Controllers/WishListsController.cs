using CozaStore.Business.Contracts;
using CozaStore.Core.DTOs;
using CozaStore.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishListsController : ControllerBase
{
    private readonly IWishListService _wishListService;
    private readonly IProductService _productService;

    public WishListsController(IWishListService wishListService, IProductService productService)
    {
        _wishListService = wishListService;
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWishList()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _wishListService.GetByUserAsync(userId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var wishListItems = new List<WishListDto>();
        foreach (var item in result.Data)
        {
            var productResult = await _productService.GetByIdAsync(item.ProductId);
            if (productResult.Success && productResult.Data != null)
            {
                var product = productResult.Data;
                wishListItems.Add(new WishListDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    Product = new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.StockQuantity,
                        ImageUrl = product.ImageUrl,
                        CategoryId = product.CategoryId,
                        CreatedDate = product.CreatedDate,
                        IsActive = product.IsActive
                    },
                    CreatedDate = item.CreatedDate
                });
            }
        }

        return Ok(wishListItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishList([FromBody] Guid productId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Ürün kontrolü
        var productResult = await _productService.GetByIdAsync(productId);
        if (!productResult.Success || productResult.Data == null)
        {
            return BadRequest(new { message = "Ürün bulunamadı." });
        }

        var wishList = new WishList
        {
            UserId = userId,
            ProductId = productId
        };

        var result = await _wishListService.AddAsync(wishList);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Ürün istek listenize eklendi." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _wishListService.RemoveAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Ürün istek listenizden çıkarıldı." });
    }
}


