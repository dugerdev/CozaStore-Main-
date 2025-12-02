using CozaStore.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CozaStoreWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        UserManager<ApplicationUser> userManager,
        ILogger<DashboardController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/dashboard/user-statistics
    /// Kullanıcı istatistiklerini getirir (Admin)
    /// </summary>
    [HttpGet("user-statistics")]
    public async Task<IActionResult> GetUserStatistics()
    {
        try
        {
            var allUsers = _userManager.Users.ToList();
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            // Son 30 günde kayıt olan kullanıcılar
            var recentRegistrations = allUsers.Count(u => u.CreatedDate >= thirtyDaysAgo);

            // Toplam kullanıcı sayısı
            var totalUsers = allUsers.Count;

            return Ok(new
            {
                RecentRegistrations = recentRegistrations,
                TotalUsers = totalUsers
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı istatistikleri yüklenirken hata oluştu");
            return StatusCode(500, new { message = "Kullanıcı istatistikleri yüklenemedi." });
        }
    }
}

