using System.Text;
using System.Text.Json;
using CozaStore.Application.Common;
using CozaStore.Application.DTOs;

namespace CozaStore.Application.Services;


/// <summary>
/// Kullanıcı adres işlemleri için API ile iletişim kurar.
/// Adres listeleme, ekleme, güncelleme, silme.
/// </summary>

public class AddressService
{
    
    private readonly HttpClient _httpClient;
    

    
    public AddressService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    

    
    /// <summary>
    /// Kullanıcının tüm adreslerini getirir
    /// </summary>
    /// <returns>Adres listesi</returns>
    
    public async Task<Result<List<AddressDto>>?> GetMyAddressesAsync()
    {
        try
        {
            // GET api/addresses
            // Not: API authentication ile kullanıcıyı otomatik tanır
            var response = await _httpClient.GetAsync("");
            
            if (!response.IsSuccessStatusCode)
            {
                return Result<List<AddressDto>>.Failure($"Adresler yüklenemedi. Status: {response.StatusCode}");
            }
            
            var content = await response.Content.ReadAsStringAsync();
            
            // API direkt liste döndürüyor (Result wrapper'ı yok)
            var addresses = JsonSerializer.Deserialize<List<AddressDto>>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            return Result<List<AddressDto>>.Success(addresses ?? new List<AddressDto>());
        }
        catch (Exception ex)
        {
            return Result<List<AddressDto>>.Failure($"Adresler yüklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// ID'ye göre tek adres getirir
    /// </summary>
    /// <param name="addressId">Adres ID'si</param>
    /// <returns>Adres detayı</returns>
    
    public async Task<Result<AddressDto>?> GetAddressByIdAsync(Guid addressId)
    {
        try
        {
            // GET api/addresses/{addressId}
            var response = await _httpClient.GetAsync(addressId.ToString());
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result<AddressDto>>(content);
        }
        catch (Exception ex)
        {
            return Result<AddressDto>.Failure($"Adres bulunamadı: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Yeni adres ekler
    /// </summary>
    /// <param name="address">Adres bilgileri</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> CreateAddressAsync(AddressDto address)
    {
        try
        {
            // API Address entity bekliyor, PascalCase property names kullan
            var addressEntity = new
            {
                Id = address.Id,
                UserId = address.UserId,
                Title = address.Title,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                District = address.District,
                PostalCode = address.PostalCode,
                Country = address.Country ?? "Turkey",
                AddressType = address.AddressType,
                IsDefault = address.IsDefault
            };
            
            var json = JsonSerializer.Serialize(addressEntity, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = null, // PascalCase kullan
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // POST api/addresses
            var response = await _httpClient.PostAsync("", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            // API 201 Created döndürüyorsa başarılı
            if (response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return Result.Success();
            }
            
            // Hata durumunda error message parse et
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    var errorObj = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    var errorMessage = "Adres eklenemedi.";
                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        errorMessage = errorObj["message"]?.ToString() ?? errorMessage;
                    }
                    
                    return Result.Failure(errorMessage);
                }
                catch
                {
                    return Result.Failure($"Adres eklenemedi. Status: {response.StatusCode}, Response: {responseContent}");
                }
            }
            
            // Diğer başarılı durumlar
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Adres eklenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Adresi günceller
    /// </summary>
    /// <param name="addressId">Güncellenecek adres ID'si</param>
    /// <param name="address">Yeni adres bilgileri</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> UpdateAddressAsync(Guid addressId, AddressDto address)
    {
        try
        {
            // API için Address entity formatında gönder (PascalCase property names)
            // Sadece değiştirilebilir property'leri gönder, BaseEntity property'leri backend tarafında yönetiliyor
            var addressEntity = new
            {
                Id = address.Id,
                UserId = address.UserId,
                Title = address.Title,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                District = address.District,
                PostalCode = address.PostalCode,
                Country = address.Country ?? "Turkey",
                AddressType = address.AddressType,
                IsDefault = address.IsDefault
            };
            
            // PascalCase property names kullan (API Address entity bekliyor)
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = null, // PascalCase kullan
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = false
            };
            
            var json = JsonSerializer.Serialize(addressEntity, jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // PUT api/addresses/{addressId}
            var response = await _httpClient.PutAsync(addressId.ToString(), content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                // 204 NoContent döndürüyor
                return Result.Success();
            }
            
            // API'den hata mesajını parse et
            try
            {
                var errorObj = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var errorMessage = "Adres güncellenemedi.";
                if (errorObj != null && errorObj.ContainsKey("message"))
                {
                    errorMessage = errorObj["message"]?.ToString() ?? errorMessage;
                }
                    
                return Result.Failure($"{errorMessage} (Status: {(int)response.StatusCode})");
            }
            catch
            {
                return Result.Failure($"Adres güncellenemedi. Status: {response.StatusCode}, Response: {responseContent}");
            }
        }
        catch (Exception ex)
        {
            return Result.Failure($"Adres güncellenemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Adresi siler
    /// </summary>
    /// <param name="addressId">Silinecek adres ID'si</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> DeleteAddressAsync(Guid addressId)
    {
        try
        {
            // DELETE api/addresses/{addressId}
            var response = await _httpClient.DeleteAsync(addressId.ToString());
            
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            
            var content = await response.Content.ReadAsStringAsync();
            
            // API'den hata mesajını parse et
            try
            {
                var errorObj = JsonSerializer.Deserialize<Dictionary<string, string>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var errorMessage = errorObj?.ContainsKey("message") == true 
                    ? errorObj["message"] 
                    : "Adres silinemedi.";
                    
                return Result.Failure(errorMessage);
            }
            catch
            {
                return Result.Failure($"Adres silinemedi. Status: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            return Result.Failure($"Adres silinemedi: {ex.Message}");
        }
    }
    

    
    /// <summary>
    /// Adresi varsayılan yapar
    /// </summary>
    /// <param name="addressId">Varsayılan yapılacak adres ID'si</param>
    /// <returns>Başarı/Hata mesajı</returns>
    
    public async Task<Result?> SetDefaultAddressAsync(Guid addressId)
    {
        try
        {
            // PUT api/addresses/{addressId}/set-default
            var response = await _httpClient.PutAsync($"{addressId}/set-default", null);
            var content = await response.Content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<Result>(content);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Varsayılan adres ayarlanamadı: {ex.Message}");
        }
    }
    
}

