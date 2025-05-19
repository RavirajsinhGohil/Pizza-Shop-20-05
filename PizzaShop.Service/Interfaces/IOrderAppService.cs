using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using static System.Collections.Specialized.BitVector32;
using Section = PizzaShop.Entity.Models.Section;

namespace PizzaShop.Service.Interfaces;

public interface IOrderAppService
{
    #region KOT Module

    Task<List<KOTOrderCardViewModel>> GetOrderDetailByCategoryId(int categoryId, bool inProgress);
    Task<bool> MarkOrderAsReady(int orderId, List<OrderDetailItem> orderDetailsIds);
    Task<bool> MarkOrderAsInProgress(int orderId, List<OrderDetailItem> orderDetailsIds);

    #endregion

    #region Table Module

    Task<OrderAppTableModuleViewModel> GetTabelModuleData();
    Task<bool> AddWaitingToken(AddWaitingTokenForTableViewModel model);
    Task<List<WaitingCustomerViewModel>> GetWaitingCustomers(int sectionId);
    Task<int?> AssignCustomerToTableAsync(AssignTableViewModel model);

    #endregion

    #region Waiting List

    Task<WaitingListViewModel> GetWaitingListSections();
    Task<List<WaitingTokenViewModel>> GetWaitingTokensBySectionId(int sectionId);
    Task<AddWaitingTokenForTableViewModel> GetWaitingTokenById(int tokenId);
    Task<bool> UpdateWaitingToken(AddWaitingTokenForTableViewModel model);
    Task<bool> DeleteWaitingToken(int tokenId);
    Task<List<Section>> GetSections();
    List<Table> GetTables(int sectionId);
    Task<CustomerViewModel?> GetCustomerByEmail(string email);
    
    #endregion

    #region Menu

    Task<CategoryItemsForOrderMenuViewModel> GetCategoryItemsForOrderMenu(string? searchText, int categoryId, int? orderId);
    void ToggleFavorite(int itemId);
    Task<ModifierSelectionModalViewModel?> GetModifiersGroupedByItemAsync(int itemId);
    Task<OrderItemForRowViewModel?> RenderOrderItemRow(RenderOrderItemRowRequest request);
    Task<OrderCustomerViewModel?> GetOrderCustomerAsync(int orderId);
    Task<bool> UpdateCustomerAsync(UpdateCustomerViewModel model);
    Task<string?> GetAdminCommentAsync(int id);
    Task<bool> SaveAdminCommentAsync(int orderDetailId, string adminComment);
    Task<string?> GetItemInstructionAsync(int orderId, int itemId);
    Task<bool> SaveItemInstructionAsync(int orderId, int itemId, string instruction);
    Task<List<SaveOrderDetailViewModel>?> SaveOrderItemsAsync(SaveOrderRequestViewModel model);
    Task<List<SaveOrderDetailViewModel>?> GetOrderDetailsById(int orderId);
    Task<List<SaveOrderDetailViewModel>?> CompleteOrderAsync(int orderId);
    Task<List<SaveOrderDetailViewModel>?> CancelOrderAsync(int orderId);

    #endregion
}
