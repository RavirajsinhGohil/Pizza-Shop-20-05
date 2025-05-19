namespace PizzaShop.Entity.ViewModel;

public class CustomersListViewModel
{
    public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    public CustomerPaginationViewModel Pagination { get; set; } = new CustomerPaginationViewModel();
}
