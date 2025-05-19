using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Interfaces;

public interface ICustomerRepository
{
    Task<List<CustomerViewModel>> GetCustomersListModel();
    Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model);
    Task<CustomersListViewModel> GetCustomersForExport(CustomerPaginationViewModel model);
    Task<CustomerViewModel> GetCustomerHistoryByCustomerId(int customerId);
}
