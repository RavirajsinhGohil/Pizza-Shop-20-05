using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Service.Interfaces;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardDataAsync(string filter);
}
