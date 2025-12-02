using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;
using CozaStore.Application.Enums;

namespace CozaStore.Application.Services;

/// <summary>
/// Sipariş işlemleri için API ile iletişim kurar.
/// Sipariş oluşturma, listeleme, detay görüntüleme.
/// </summary>
public class OrderService
{
    
    private readonly HttpClient _httpClient;
    

    
    public OrderService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    

    
    /// <summary>
    /// Kullanıcının tüm siparişlerini getirir
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <returns>Sipariş listesi</returns>
    
    public async Task<Result<List<OrderDto>>?> GetMyOrdersAsync(string userId)
    {
        try
        {
            // GET api/orders/user/{userId}
            var response = await _httpClient.GetAsync($"user/{userId}");
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result<List<OrderDto>>>(content);
        }
        catch (Exception ex)
        {
            return Result<List<OrderDto>>.Failure($"Siparişler yüklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Sipariş detayını getirir
    /// </summary>
    /// <param name="orderId">Sipariş ID'si</param>
    /// <returns>Sipariş detayı</returns>
    
    public async Task<Result<OrderDto>?> GetOrderByIdAsync(Guid orderId)
    {
        try
        {
            // GET api/orders/{orderId}
            var response = await _httpClient.GetAsync(orderId.ToString());
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result<OrderDto>>(content);
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Sipariş bulunamadı: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Yeni sipariş oluşturur
    /// </summary>
    /// <param name="request">Sipariş bilgileri (Adres, ürünler, ödeme yöntemi)</param>
    /// <returns>Oluşturulan sipariş</returns>
    
    public async Task<Result<OrderDto>?> CreateOrderAsync(CreateOrderRequestDto request)
    {
        try
        {
            // API için doğru formatta serialize et (PascalCase property names)
            // API CozaStore.Core.DTOs.CreateOrderRequestDto bekliyor
            // PaymentMethod nullable olmalı (API PaymentMethod? bekliyor)
            // API CozaStore.Entities.Enums.PaymentMethod bekliyor, biz CozaStore.Application.Enums.PaymentMethod kullanıyoruz
            // Enum değerini integer olarak gönder (aynı değerler: 0=CreditCard, 1=BankTransfer, vb.)
            var apiRequest = new
            {
                ShippingAddressId = request.ShippingAddressId,
                BillingAddressId = request.BillingAddressId,
                PaymentMethod = (int)request.PaymentMethod, // Integer olarak gönder
                ShippingCost = request.ShippingCost,
                TaxAmount = request.TaxAmount,
                Notes = request.Notes,
                OrderDetails = request.OrderDetails.Select(item => new
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null, // PascalCase kullan (API bekliyor)
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                // Enum'ları integer olarak serialize et (API CozaStore.Entities.Enums.PaymentMethod bekliyor, integer değer göndermeliyiz)
            };

            var json = JsonSerializer.Serialize(apiRequest, jsonOptions);
            
            // Debug: JSON'u logla (geliştirme için)
            System.Diagnostics.Debug.WriteLine($"Order Request JSON: {json}");
            System.Console.WriteLine($"Order Request JSON: {json}"); // Console'a da yaz
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // POST api/orders
            var response = await _httpClient.PostAsync("", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                return Result<OrderDto>.Failure($"Sipariş oluşturulamadı: {responseContent}");
            }
            
            // API CreatedAtAction ile direkt OrderDto döndürüyor (Result wrapper yok)
            // 201 Created durumunda response body'de OrderDto var
            var deserializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            try
            {
                // API direkt OrderDto döndürüyor, Result wrapper'ı yok
                var orderDto = JsonSerializer.Deserialize<OrderDto>(responseContent, deserializeOptions);
                if (orderDto != null)
                {
                    return Result<OrderDto>.Success(orderDto);
                }
                return Result<OrderDto>.Failure("Sipariş oluşturuldu ama response parse edilemedi.");
            }
            catch (Exception ex)
            {
                return Result<OrderDto>.Failure($"Response parse hatası: {ex.Message}. Response: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            return Result<OrderDto>.Failure($"Sipariş oluşturulamadı: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Sipariş ödeme durumunu günceller
    /// </summary>
    /// <param name="orderId">Sipariş ID'si</param>
    /// <param name="paymentStatus">Yeni ödeme durumu</param>
    /// <returns>Başarı/Hata mesajı</returns>
    public async Task<Result?> UpdatePaymentStatusAsync(Guid orderId, PaymentStatus paymentStatus)
    {
        try
        {
            // API için PaymentStatus'u integer olarak gönder
            var json = JsonSerializer.Serialize((int)paymentStatus);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // PUT api/orders/{orderId}/payment-status
            var response = await _httpClient.PutAsync($"{orderId}/payment-status", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            System.Console.WriteLine($"UpdatePaymentStatus Response: StatusCode={response.StatusCode}, Content={responseContent}");
            
            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine($"❌ UpdatePaymentStatus failed: {response.StatusCode} - {responseContent}");
                return Result.Failure($"Ödeme durumu güncellenemedi: {response.StatusCode} - {responseContent}");
            }
            
            System.Console.WriteLine($"✅ UpdatePaymentStatus success: OrderId={orderId}, PaymentStatus={paymentStatus}");
            return Result.Success();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ UpdatePaymentStatus exception: {ex.Message}");
            return Result.Failure($"Ödeme durumu güncellenemedi: {ex.Message}");
        }
    }

    /// <summary>
    /// Siparişi iptal eder
    /// </summary>
    /// <param name="orderId">İptal edilecek sipariş ID'si</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> CancelOrderAsync(Guid orderId)
    {
        try
        {
            // DELETE api/orders/{orderId}
            var response = await _httpClient.DeleteAsync(orderId.ToString());
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result>(content);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Sipariş iptal edilemedi: {ex.Message}");
        }
    }
    
}
