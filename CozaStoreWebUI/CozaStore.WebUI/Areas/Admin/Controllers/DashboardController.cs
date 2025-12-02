using CozaStore.Application.Services;
using CozaStore.Application.Enums;
using CozaStore.WebUI.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CozaStore.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AdminOrderService _orderService;
        private readonly AdminDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            AdminOrderService orderService,
            AdminDashboardService dashboardService,
            ILogger<DashboardController> logger)
        {
            _orderService = orderService;
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();

            try
            {
                // Siparişleri getir
                var ordersResult = await _orderService.GetAllAsync();
                
                if (ordersResult?.IsSuccess == true && ordersResult.Data != null)
                {
                    var orders = ordersResult.Data;
                    var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

                    // Son 30 günde bekleyen/yeni siparişler
                    viewModel.NewOrdersCount = orders
                        .Count(o => o.OrderDate >= thirtyDaysAgo && 
                                   (o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing));

                    // Bounce Rate hesaplama (sipariş tamamlama oranı)
                    // Tamamlanan siparişler / Toplam siparişler * 100
                    var totalOrders = orders.Count;
                    var completedOrders = orders.Count(o => o.Status == OrderStatus.Delivered);
                    if (totalOrders > 0)
                    {
                        var completionRate = (double)completedOrders / totalOrders * 100;
                        viewModel.BounceRate = Math.Round(100 - completionRate, 1); // Bounce = tamamlanmayan
                    }
                    else
                    {
                        viewModel.BounceRate = 0;
                    }

                    // Benzersiz ziyaretçiler için sipariş yapan kullanıcı sayısı
                    viewModel.UniqueVisitorsCount = orders
                        .Select(o => o.UserId)
                        .Distinct()
                        .Count();
                }

                // Kullanıcı istatistiklerini getir
                var userStatsResult = await _dashboardService.GetUserStatisticsAsync();
                if (userStatsResult?.IsSuccess == true && userStatsResult.Data != null)
                {
                    viewModel.UserRegistrationsCount = userStatsResult.Data.RecentRegistrations;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dashboard verileri yüklenirken hata oluştu");
                // Hata durumunda varsayılan değerler kullanılacak
            }

            return View(viewModel);
        }
    }
}
