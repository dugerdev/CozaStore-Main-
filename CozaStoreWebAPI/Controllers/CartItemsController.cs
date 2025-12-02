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
public class CartItemsController : ControllerBase
{
    private readonly ICartItemService _cartItemService;
    private readonly IProductService _productService;
    private readonly ILogger<CartItemsController> _logger;

    public CartItemsController(ICartItemService cartItemService, IProductService productService, ILogger<CartItemsController> logger)
    {
        _cartItemService = cartItemService;
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Test endpoint - Token'ın doğru gönderilip gönderilmediğini kontrol eder
    /// </summary>
    [HttpGet("test")]
    public IActionResult TestToken()
    {
        var authHeader = Request.Headers["Authorization"].ToString();
        var hasAuthHeader = !string.IsNullOrEmpty(authHeader);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        
        return Ok(new
        {
            HasAuthHeader = hasAuthHeader,
            AuthHeader = authHeader?.Substring(0, Math.Min(100, authHeader?.Length ?? 0)) + "...",
            IsAuthenticated = isAuthenticated,
            UserId = userId,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { 
                IsSuccess = false, 
                ErrorMessage = "Giriş yapmanız gerekiyor.",
                Data = (object?)null
            });
        }

        var result = await _cartItemService.GetByUserAsync(userId);
        
        if (!result.Success)
        {
            return BadRequest(new { 
                IsSuccess = false, 
                ErrorMessage = result.Message,
                Data = (object?)null
            });
        }

        var cartItems = new List<CartItemDto>();
        foreach (var item in result.Data)
        {
            var productResult = await _productService.GetByIdAsync(item.ProductId);
            if (productResult.Success && productResult.Data != null)
            {
                var product = productResult.Data;
                cartItems.Add(new CartItemDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    ProductPrice = product.Price,
                    ProductImageUrl = product.ImageUrl,
                    Quantity = item.Quantity,
                    SubTotal = product.Price * item.Quantity
                });
            }
        }

        return Ok(new { 
            IsSuccess = true, 
            ErrorMessage = (string?)null,
            Data = cartItems
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
    {
        // Token kontrolü - debugging için
        var authHeader = Request.Headers["Authorization"].ToString();
        var hasAuthHeader = !string.IsNullOrEmpty(authHeader);
        
        // Tüm header'ları logla
        var allHeaders = string.Join(", ", Request.Headers.Select(h => $"{h.Key}={h.Value}"));
        _logger.LogError("🔍 AddToCart called. HasAuthHeader: {HasAuthHeader}, AuthHeader: {AuthHeader}, User: {User}, AllHeaders: {AllHeaders}", 
            hasAuthHeader, 
            authHeader?.Substring(0, Math.Min(100, authHeader?.Length ?? 0)) + "...", 
            User.Identity?.Name,
            allHeaders?.Substring(0, Math.Min(500, allHeaders?.Length ?? 0)) + "...");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogError("❌ UserId is null or empty. User.Identity.IsAuthenticated: {IsAuthenticated}, Claims: {Claims}", 
                User.Identity?.IsAuthenticated, string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}")));
            
            return Unauthorized(new { 
                IsSuccess = false, 
                ErrorMessage = "Giriş yapmanız gerekiyor.",
                Data = (object?)null
            });
        }
        
        _logger.LogInformation("✅ User authenticated. UserId: {UserId}", userId);

        // Ürün kontrolü
        var productResult = await _productService.GetByIdAsync(request.ProductId);
        if (!productResult.Success || productResult.Data == null)
        {
            return BadRequest(new { 
                IsSuccess = false, 
                ErrorMessage = "Ürün bulunamadı.",
                Data = (object?)null
            });
        }

        var cartItem = new CartItem
        {
            UserId = userId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        var result = await _cartItemService.AddAsync(cartItem);
        
        if (!result.Success)
        {
            return BadRequest(new { 
                IsSuccess = false, 
                ErrorMessage = result.Message,
                Data = (object?)null
            });
        }

        return Ok(new { 
            IsSuccess = true, 
            ErrorMessage = (string?)null,
            Data = (object?)null
        });
    }

    [HttpPut("{id}/quantity")]
    public async Task<IActionResult> UpdateQuantity(Guid id, [FromBody] UpdateCartQuantityRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { 
                IsSuccess = false, 
                ErrorMessage = "Giriş yapmanız gerekiyor.",
                Data = (object?)null
            });
        }

        var result = await _cartItemService.UpdateQuantityAsync(id, request.Quantity);
        
        if (!result.Success)
        {
            return BadRequest(new { 
                IsSuccess = false, 
                ErrorMessage = result.Message,
                Data = (object?)null
            });
        }

        return Ok(new { 
            IsSuccess = true, 
            ErrorMessage = (string?)null,
            Data = (object?)null
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { 
                IsSuccess = false, 
                ErrorMessage = "Giriş yapmanız gerekiyor.",
                Data = (object?)null
            });
        }

        var result = await _cartItemService.RemoveAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { 
                IsSuccess = false, 
                ErrorMessage = result.Message,
                Data = (object?)null
            });
        }

        return Ok(new { 
            IsSuccess = true, 
            ErrorMessage = (string?)null,
            Data = (object?)null
        });
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { 
                IsSuccess = false, 
                ErrorMessage = "Giriş yapmanız gerekiyor.",
                Data = (object?)null
            });
        }

        var result = await _cartItemService.ClearAsync(userId);
        
        if (!result.Success)
        {
            return BadRequest(new { 
                IsSuccess = false, 
                ErrorMessage = result.Message,
                Data = (object?)null
            });
        }

        return Ok(new { 
            IsSuccess = true, 
            ErrorMessage = (string?)null,
            Data = (object?)null
        });
    }
}


