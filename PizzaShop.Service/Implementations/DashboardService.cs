using PizzaShop.Entity.ViewModel;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }
    
    public async Task<DashboardViewModel> GetDashboardDataAsync(string filter)
    {
        var dashboardData = await _dashboardRepository.GetDashboardDataAsync(filter);
        return dashboardData;
    }
}
