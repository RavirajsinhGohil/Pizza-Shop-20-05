using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Helper;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

// [Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string filter = "Current Month")
    {

        DashboardViewModel? model = await _dashboardService.GetDashboardDataAsync(filter);
        return View(model);
    }
}
