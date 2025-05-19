using PizzaShop.Repository.GetDataFromToken;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Service.Interfaces;

public interface ITaxService
{
    TaxListPaginationViewModel GetTaxList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    Task<AuthResponse> AddTax(AddTaxViewModel model);
    Task<AuthResponse> EditTax(AddTaxViewModel model);
    Task<AuthResponse> DeleteTax(int id);
    Task<List<TaxViewModel>> GetTaxesByOrderId(int orderId);
}
