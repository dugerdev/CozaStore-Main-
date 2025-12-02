using CozaStore.Business.Contracts;
using CozaStore.Core.DTOs;
using CozaStore.Entities.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IProductService _productService;

    public ReviewsController(IReviewService reviewService, IProductService productService)
    {
        _reviewService = reviewService;
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reviewService.GetAllAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var reviews = result.Data.Select(r => new ReviewDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            UserId = r.UserId,
            Title = r.Title,
            Comment = r.Comment,
            Rating = r.Rating,
            IsApproved = r.IsApproved,
            CreatedDate = r.CreatedDate
        }).ToList();

        return Ok(reviews);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var result = await _reviewService.GetByProductAsync(productId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        // Sadece onaylanmış yorumları döndür
        var reviews = result.Data
            .Where(r => r.IsApproved)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                Title = r.Title,
                Comment = r.Comment,
                Rating = r.Rating,
                IsApproved = r.IsApproved,
                CreatedDate = r.CreatedDate
            }).ToList();

        return Ok(reviews);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPending()
    {
        var result = await _reviewService.GetPendingAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var reviews = result.Data.Select(r => new ReviewDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            UserId = r.UserId,
            Title = r.Title,
            Comment = r.Comment,
            Rating = r.Rating,
            IsApproved = r.IsApproved,
            CreatedDate = r.CreatedDate
        }).ToList();

        return Ok(reviews);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Ürün kontrolü
        var productResult = await _productService.GetByIdAsync(request.ProductId);
        if (!productResult.Success || productResult.Data == null)
        {
            return BadRequest(new { message = "Ürün bulunamadı." });
        }

        var review = new Review
        {
            ProductId = request.ProductId,
            UserId = userId,
            Title = request.Title,
            Comment = request.Comment,
            Rating = request.Rating,
            IsApproved = false // Admin onayı bekliyor
        };

        var result = await _reviewService.AddAsync(review);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Yorumunuz gönderildi. Onay bekleniyor." });
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _reviewService.ApproveAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Yorum onaylandı." });
    }

    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(Guid id)
    {
        var result = await _reviewService.RejectAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = "Yorum reddedildi." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _reviewService.DeleteAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }
}


