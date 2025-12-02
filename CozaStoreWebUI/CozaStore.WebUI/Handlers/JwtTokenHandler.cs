using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace CozaStore.WebUI.Handlers;

/// <summary>
/// HttpClient isteklerine JWT token'ı Authorization header'ı olarak ekler
/// </summary>
public class JwtTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<JwtTokenHandler> _logger;

    public JwtTokenHandler(IHttpContextAccessor httpContextAccessor, ILogger<JwtTokenHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // HttpContext'ten cookie'yi al
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Tüm cookie'leri logla (debug için)
            var allCookies = string.Join(", ", httpContext.Request.Cookies.Select(c => $"{c.Key}={c.Value?.Substring(0, Math.Min(20, c.Value?.Length ?? 0))}..."));
            _logger.LogDebug("Available cookies: {Cookies}", allCookies);
            
            var token = httpContext.Request.Cookies["AuthToken"];
            
            if (!string.IsNullOrEmpty(token))
            {
                // Authorization header'ına Bearer token olarak ekle
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                // Header'ın gerçekten eklendiğini doğrula
                var addedHeader = request.Headers.Authorization?.ToString();
                _logger.LogError("JWT token added to request: {RequestUri}, Token length: {TokenLength}, Token preview: {TokenPreview}, AddedHeader: {AddedHeader}", 
                    request.RequestUri, token.Length, token.Substring(0, Math.Min(50, token.Length)) + "...", addedHeader?.Substring(0, Math.Min(100, addedHeader?.Length ?? 0)) + "...");
            }
            else
            {
                _logger.LogError("AuthToken cookie not found for request: {RequestUri}. Available cookies: {Cookies}", 
                    request.RequestUri, allCookies);
            }
        }
        else
        {
            _logger.LogError("❌ HttpContext is null for request: {RequestUri}", request.RequestUri);
        }

        // Header'ın gerçekten eklendiğini doğrula (göndermeden önce)
        var authHeaderBeforeSend = request.Headers.Authorization?.ToString();
        _logger.LogError("🔍 Before SendAsync - RequestUri: {RequestUri}, Authorization header: {Header}, HasHeader: {HasHeader}", 
            request.RequestUri, 
            authHeaderBeforeSend?.Substring(0, Math.Min(100, authHeaderBeforeSend?.Length ?? 0)) + "...",
            request.Headers.Authorization != null);

        var response = await base.SendAsync(request, cancellationToken);
        
        // Response'u logla
        _logger.LogError("📥 Response received - StatusCode: {StatusCode}, RequestUri: {RequestUri}, ReasonPhrase: {ReasonPhrase}", 
            response.StatusCode, request.RequestUri, response.ReasonPhrase);
        
        // Response header'larını da logla (debug için)
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("❌ Error response content: {Content}", responseContent?.Substring(0, Math.Min(500, responseContent?.Length ?? 0)) + "...");
        }
        
        return response;
    }
}

