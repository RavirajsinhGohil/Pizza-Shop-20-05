using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Interfaces;

public interface IOrderAppRepository
{
    #region KOT

    Task<List<Order>> GetOrderDetailsByMenuCategory(int menuCategoryId);
    Task<bool> MarkOrderAsReady(int orderId, List<OrderDetailItem> orderDetailsIds);
    Task<bool> MarkOrderAsInProgress(int orderId, List<OrderDetailItem> orderDetailsIds);

    #endregion

    #region Waiting List

    Task<bool> AddWaitingToken(AddWaitingTokenForTableViewModel model);
    Task<List<Waitingticket>> GetWaitingCustomers(int sectionId);
    Task<List<Table>> GetTableListByTableIds(List<int> Ids);
    Task<Waitingticket> GetWaitingCustomerByWaitingTokenId(int waitingTokenId);
    Task<bool> UpdateWaitingToken(Waitingticket? waitingTicket);
    Task<bool> AddCustomerForWaitingList(Customer customer);
    Task<Order> GetOrderByCustomerId(int customerId);
    Task<bool> AddOrder(Order order);
    Task<bool> UpdateOrder(Order order);
    Task<bool> AddTableOrderMapping(int orderId, int tableId);
    Task<List<Waitingticket>> GetWaitingTokens();
    Task<List<Waitingticket>> GetWaitingTokensBySectionId(int sectionId);
    Task<AddWaitingTokenForTableViewModel> GetWaitingTokenForUpdate(int tokenId);
    Task<List<Section>> GetSections();
    List<Table> GetTables(int sectionId);
    Task<Customer?> GetCustomerByEmail(string email);

    #endregion

    #region Menu

    List<Menucategory> GetAllCategories();
    List<Item> GetFilteredItems(int categoryId, string? searchText);
    string GetModifierNameById(int? modifierId);
    void ToggleFavorite(int itemId);
    Task<ModifierSelectionModalViewModel> GetModifiersGroupedByItemAsync(int itemId);
    Task<OrderCustomerViewModel?> GetOrderCustomerAsync(int orderId);
    Task<bool> UpdateCustomerAsync(UpdateCustomerViewModel vm);
    Task<string?> GetAdminCommentAsync(int orderDetailId);
    Task<bool> SaveAdminCommentAsync(int orderDetailId, string adminComment);
    Task<string?> GetItemInstructionAsync(int orderId, int itemId);
    Task<bool> SaveItemInstructionAsync(int orderId, int itemId, string instruction);
    string? GetSectionName(int? orderId);
    List<string> GetTableNames(int? orderId);
    List<Taxesandfee> GetEnabledTaxes();
    Task<List<Orderdetail>> SaveOrderItemsAsync(SaveOrderRequestViewModel model);
    Task<Order> GetOrderByIdAsync(int orderId);
    Task<bool> DeleteOrderDetailByCompositeMatchAsync(DeleteOrderDetailRequest request);
    Task<int?> GetReadyQuantity(int itemId, int orderId);
    Task<List<SaveOrderDetailViewModel>?> CompleteOrderItemsAsync(int orderId);
    Task<List<SaveOrderDetailViewModel>?> CancelOrderItemsAsync(int orderId);

    #endregion
}
