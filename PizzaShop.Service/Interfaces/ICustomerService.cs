using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Service.Interfaces;

public interface ICustomerService
{
    Task<CustomersListViewModel> GetCustomersListModel();
    Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model);
    Task<byte[]> ExportDataInExcel (CustomerPaginationViewModel viewModel);
    Task<CustomerViewModel> GetCustomerHistoryByCustomerId(int customerId);
}
