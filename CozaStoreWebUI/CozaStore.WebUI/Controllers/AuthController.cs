using CozaStore.Application.DTOs;
using CozaStore.Application.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CozaStore.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(AuthService authService, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _authService = authService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Login sayfasını gösterir
        /// </summary>
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["Title"] = "Login";
            ViewData["ActiveMenu"] = "";
            ViewData["HeaderClass"] = "header-v4";
            ViewData["MenuShadow"] = "yes";
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Login işlemini gerçekleştirir
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
        {
            ViewData["Title"] = "Login";
            ViewData["ActiveMenu"] = "";
            ViewData["HeaderClass"] = "header-v4";
            ViewData["MenuShadow"] = "yes";
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doldurun.";
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result?.IsSuccess == true && result.Data != null)
            {
                // JWT Token'dan claims çıkar
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(result.Data.AccessToken);
                var claims = jwtToken.Claims.ToList();

                // ClaimsIdentity oluştur
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = result.Data.ExpiresAt,
                    AllowRefresh = true
                };

                // Cookie authentication ile sign in
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Token'ı da cookie'ye kaydet (API istekleri için)
                Response.Cookies.Append("AuthToken", result.Data.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Development için false, production'da true olmalı
                    SameSite = SameSiteMode.Lax, // Lax yapıldı - Admin area için gerekli
                    Path = "/", // Tüm path'ler için geçerli
                    Expires = result.Data.ExpiresAt
                });
                
                _logger.LogInformation("AuthToken cookie set. Expires: {ExpiresAt}", result.Data.ExpiresAt);

                TempData["SuccessMessage"] = "Giriş başarılı!";
                _logger.LogInformation("User {Email} logged in successfully.", model.Email);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Giriş başarısız. Email veya şifre hatalı.";
            _logger.LogWarning("Login failed for user {Email}. Error: {Error}", model.Email, TempData["ErrorMessage"]);
            return View(model);
        }

        /// <summary>
        /// Register sayfasını gösterir
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Title"] = "Register";
            ViewData["ActiveMenu"] = "";
            ViewData["HeaderClass"] = "header-v4";
            ViewData["MenuShadow"] = "yes";
            return View();
        }

        /// <summary>
        /// Register işlemini gerçekleştirir
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            ViewData["Title"] = "Register";
            ViewData["ActiveMenu"] = "";
            ViewData["HeaderClass"] = "header-v4";
            ViewData["MenuShadow"] = "yes";

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doldurun.";
                return View(model);
            }

            // Şifre kontrolü
            if (model.Password != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "Şifreler eşleşmiyor.";
                return View(model);
            }

            var result = await _authService.RegisterAsync(model);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                _logger.LogInformation("User {Email} registered successfully.", model.Email);
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = result?.ErrorMessage ?? "Kayıt başarısız. Lütfen tekrar deneyin.";
            _logger.LogWarning("Registration failed for user {Email}. Error: {Error}", model.Email, TempData["ErrorMessage"]);
            return View(model);
        }

        /// <summary>
        /// Logout işlemini gerçekleştirir
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Cookie authentication'dan sign out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Token cookie'sini sil
            Response.Cookies.Delete("AuthToken");

            // API'ye logout isteği gönder (opsiyonel)
            await _authService.LogoutAsync();

            TempData["SuccessMessage"] = "Çıkış yapıldı.";
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Access Denied sayfası
        /// </summary>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Access Denied";
            ViewData["ActiveMenu"] = "";
            ViewData["HeaderClass"] = "header-v4";
            ViewData["MenuShadow"] = "yes";
            return View();
        }
    }
}
