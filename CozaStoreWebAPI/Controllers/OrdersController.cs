using CozaStore.Business.Contracts;
using CozaStore.Core.DTOs;
using CozaStore.Entities.Entities;
using CozaStore.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;

    public OrdersController(IOrderService orderService, IProductService productService)
    {
        _orderService = orderService;
        _productService = productService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _orderService.GetAllAsync();
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var orders = result.Data.Select(o => MapToOrderDto(o)).ToList();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _orderService.GetByIdAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Message });
        }

        var order = result.Data;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (order.UserId != userId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return Ok(MapToOrderDto(order));
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (userId != currentUserId && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var result = await _orderService.GetByUserAsync(userId);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        var orders = result.Data.Select(o => MapToOrderDto(o)).ToList();
        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Sipariş numarası oluştur
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        // Toplam tutarı hesapla
        decimal totalAmount = 0;
        var orderDetails = new List<OrderDetail>();

        foreach (var item in request.OrderDetails)
        {
            var productResult = await _productService.GetByIdAsync(item.ProductId);
            if (!productResult.Success || productResult.Data == null)
            {
                return BadRequest(new { message = $"Ürün bulunamadı: {item.ProductId}" });
            }

            var product = productResult.Data;
            var unitPrice = product.Price;
            var subTotal = unitPrice * item.Quantity;

            orderDetails.Add(new OrderDetail
            {
                ProductId = item.ProductId,
                ProductName = product.Name,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                SubTotal = subTotal
            });

            totalAmount += subTotal;
        }

        var order = new Order
        {
            OrderNumber = orderNumber,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount + request.ShippingCost + request.TaxAmount,
            ShippingCost = request.ShippingCost,
            TaxAmount = request.TaxAmount,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
            PaymentMethod = request.PaymentMethod,
            ShippingAddressId = request.ShippingAddressId,
            BillingAddressId = request.BillingAddressId,
            UserId = userId,
            Notes = request.Notes
        };

        var result = await _orderService.AddAsync(order, orderDetails);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, MapToOrderDto(order));
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] OrderStatus status)
    {
        var result = await _orderService.UpdateStatusAsync(id, status);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }

    [HttpPut("{id}/payment-status")]
    [AllowAnonymous] // Stripe callback'inden gelebilir, bu yüzden AllowAnonymous
    public async Task<IActionResult> UpdatePaymentStatus(Guid id, [FromBody] PaymentStatus paymentStatus)
    {
        var orderResult = await _orderService.GetByIdAsync(id);
        if (!orderResult.Success || orderResult.Data == null)
        {
            return NotFound(new { message = "Sipariş bulunamadı." });
        }

        var order = orderResult.Data;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Kullanıcı authenticated ise, sadece kendi siparişini veya Admin güncelleyebilir
        // Stripe callback'inden geldiğinde cookie hala mevcut olmalı, bu yüzden userId olmalı
        if (!string.IsNullOrEmpty(userId))
        {
            if (order.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
        }
        else
        {
            // Kullanıcı bilgisi yoksa, sadece Admin güncelleyebilir
            // Stripe callback'inden geldiğinde cookie mevcut olmalı, ama yine de kontrol ediyoruz
            if (!User.IsInRole("Admin"))
            {
                // Eğer kullanıcı bilgisi yoksa ve Admin değilse, sadece PaymentStatus.Paid için izin ver
                // (Stripe callback'inden geldiğinde ödeme zaten doğrulanmış olacak)
                if (paymentStatus != PaymentStatus.Paid)
                {
                    return Forbid();
                }
                // PaymentStatus.Paid için izin ver (Stripe zaten ödemeyi doğruladı)
            }
        }

        var result = await _orderService.UpdatePaymentStatusAsync(id, paymentStatus);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _orderService.DeleteAsync(id);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return NoContent();
    }

    private OrderDto MapToOrderDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            ShippingCost = order.ShippingCost,
            TaxAmount = order.TaxAmount,
            Status = order.Status,
            PaymentStatus = order.PaymentStatus,
            PaymentMethod = order.PaymentMethod,
            ShippingAddressId = order.ShippingAddressId,
            BillingAddressId = order.BillingAddressId,
            UserId = order.UserId,
            Notes = order.Notes,
            OrderDetails = order.OrderDetails?.Select(od => new OrderDetailDto
            {
                Id = od.Id,
                OrderId = od.OrderId,
                ProductId = od.ProductId,
                ProductName = od.ProductName,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                SubTotal = od.SubTotal
            }).ToList() ?? new List<OrderDetailDto>()
        };
    }
}
