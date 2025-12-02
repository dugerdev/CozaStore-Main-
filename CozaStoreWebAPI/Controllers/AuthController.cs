using CozaStore.Core.DTOs;
using CozaStore.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/auth/register
    /// Yeni kullanıcı kaydı
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { 
                    IsSuccess = false, 
                    ErrorMessage = "Email ve şifre gereklidir." 
                });
            }

            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest(new { 
                    IsSuccess = false, 
                    ErrorMessage = "Şifreler eşleşmiyor." 
                });
            }

            // Kullanıcı zaten var mı kontrol et
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { 
                    IsSuccess = false, 
                    ErrorMessage = "Bu email adresi zaten kullanılıyor." 
                });
            }

            // Yeni kullanıcı oluştur
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true // Development için otomatik onaylı
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { 
                    IsSuccess = false, 
                    ErrorMessage = $"Kayıt başarısız: {errors}" 
                });
            }

            // Varsayılan olarak User rolü ver
            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("User {Email} registered successfully.", request.Email);

            return Ok(new { 
                IsSuccess = true, 
                ErrorMessage = (string?)null 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", request.Email);
            return StatusCode(500, new { 
                IsSuccess = false, 
                ErrorMessage = "Kayıt sırasında bir hata oluştu." 
            });
        }
    }

    /// <summary>
    /// POST /api/auth/login
    /// Kullanıcı girişi ve JWT token döndürme
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { 
                    IsSuccess = false, 
                    ErrorMessage = "Email ve şifre gereklidir.",
                    Data = (object?)null
                });
            }

            // Kullanıcıyı bul
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new { 
                    IsSuccess = false, 
                    ErrorMessage = "Email veya şifre hatalı.",
                    Data = (object?)null
                });
            }

            // Şifre kontrolü
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!passwordCheck.Succeeded)
            {
                return Unauthorized(new { 
                    IsSuccess = false, 
                    ErrorMessage = "Email veya şifre hatalı.",
                    Data = (object?)null
                });
            }

            // JWT Token oluştur
            var token = GenerateJwtToken(user);
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");
            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            _logger.LogInformation("User {Email} logged in successfully.", request.Email);

            return Ok(new { 
                IsSuccess = true, 
                ErrorMessage = (string?)null,
                Data = new
                {
                    AccessToken = token,
                    RefreshToken = string.Empty, // Refresh token implementasyonu için
                    ExpiresAt = expiresAt
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return StatusCode(500, new { 
                IsSuccess = false, 
                ErrorMessage = "Giriş sırasında bir hata oluştu.",
                Data = (object?)null
            });
        }
    }

    /// <summary>
    /// POST /api/auth/logout
    /// Kullanıcı çıkışı (token'ı geçersiz kılma için kullanılabilir)
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            // JWT token'lar stateless olduğu için sunucu tarafında özel bir işlem yapmaya gerek yok
            // Client tarafında token silinir
            return Ok(new { 
                IsSuccess = true, 
                ErrorMessage = (string?)null 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { 
                IsSuccess = false, 
                ErrorMessage = "Çıkış sırasında bir hata oluştu." 
            });
        }
    }

    /// <summary>
    /// JWT Token oluşturur
    /// </summary>
    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey bulunamadı.");
        var issuer = jwtSettings["Issuer"] ?? "CozaStoreAPI";
        var audience = jwtSettings["Audience"] ?? "CozaStoreClient";
        var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"] ?? "60");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        // Kullanıcının rollerini ekle
        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

