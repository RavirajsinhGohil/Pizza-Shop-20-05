using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardViewModel> GetDashboardDataAsync(string filter);
}
