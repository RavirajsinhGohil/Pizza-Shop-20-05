using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Interfaces;

public interface IOrderRepository
{
    Task<OrdersViewModel> GetOrderById(int orderId);
    Task<List<OrdersViewModel>> GetOrdersListModel();
    Task<OrdersListViewModel> GetOrderByPaginationAsync(string searchTerm, int page, int pageSize, string SortBy,  string SortOrder, string statusLog, string timeLog, string fromDate, string toDate);
    Task<OrdersListViewModel> GetOrdersForExport(string searchTerm, string statusLog, string timeLog);
    Task<CustomerViewModel> GetCustomerById(int customerId);
    Task<List<TableViewModel>> GetTablesByOrderId (int tableId);
    Task<List<ItemViewModel>> GetItemsByOrderId(int orderId);
    Task<List<ModifiersViewModel>> GetModifiersByOrderId(int orderId);
}