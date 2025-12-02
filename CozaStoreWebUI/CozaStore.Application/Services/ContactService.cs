using System.Text;
using System.Text.Json;

namespace CozaStore.Application.Services;

/// <summary>
/// İletişim formu işlemleri için API ile iletişim kurar.
/// </summary>
public class ContactService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = null
    };

    public ContactService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// İletişim formu mesajı gönderir
    /// </summary>
    public async Task<ContactResult> SendMessageAsync(string email, string message)
    {
        try
        {
            var requestData = new
            {
                Email = email,
                Message = message
            };

            var json = JsonSerializer.Serialize(requestData, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("", content);

            if (response.IsSuccessStatusCode)
            {
                return new ContactResult
                {
                    IsSuccess = true,
                    Message = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız."
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Hata mesajını parse etmeye çalış
            string errorMessage = "Mesaj gönderilemedi. Lütfen tekrar deneyin.";
            try
            {
                if (!string.IsNullOrEmpty(errorContent))
                {
                    var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent, JsonOptions);
                    if (errorObj.TryGetProperty("message", out var messageProp))
                    {
                        errorMessage = messageProp.GetString() ?? errorMessage;
                    }
                }
            }
            catch
            {
                // Parse edilemezse varsayılan mesajı kullan
            }
            
            return new ContactResult
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        catch (Exception ex)
        {
            return new ContactResult
            {
                IsSuccess = false,
                Message = $"Bir hata oluştu: {ex.Message}"
            };
        }
    }
}

/// <summary>
/// Contact işlemlerinin sonucunu temsil eder
/// </summary>
public class ContactResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}

