using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Service.Interfaces;

public interface IMenuService
{
    Task<List<CategoryViewModel>> GetCategories();
    Task<CategoryViewModel> GetCategorybyId(int categoryId);
    Task<MenuItemViewModel> GetMenuModel(int categoryId);
    Task<bool> AddCategory(CategoryViewModel model);
    Task<MenuItemViewModel> GetItemsByCategoryAsync(int categoryid, string searchTerm, int page, int pageSize);
    Task<MenuItemViewModel> GetModifierItemsByModifierGroupAsync(int categoryid, string searchTerm, int page, int pageSize);
    Menucategory GetCategoryForEdit(int categoryId);
    Task<bool> EditCategory(CategoryViewModel model);
    Task<bool> DeleteCategory(int categoryId);
    Task<bool> AddItem(ItemViewModel model);
    Task<MenuItemViewModel> GetMenuItemForEdit(int id);
    Task<bool> UpdatedMenuItem(int id, MenuItemViewModel model);
    bool DeleteItem(int itemId);
    Task<List<ModifiersViewModel>> GetModifiersForExistingModifiersForAdd(int pageNumber, int pageSize);
    Task<MenuItemViewModel> GetMenuItemModelForAddExistingModifier(int pageNumber, int pageSize);
    Task<int> GetTotalModifiersCount();
    Task<bool> AddModifierGroup(ModifierGroupViewModel model);
    // bool UpdateModifierGroup(ModifierGroupViewModel model);
    bool DeleteModifierGroup(int modifierGroupId);
    Task<bool> AddModifierinGroups(ModifiersViewModel model);
    Task<bool> addExistingModifiersForEdit(int modifierGroupId, string name, string description, List<int> selectedModifiers);
    Task<ModifierGroupViewModel> GetModifierGroupForEdit(int id);
    Task<MenuItemViewModel> GetDataForEditModifier(int id);
    Task<bool> UpdateModifier(MenuItemViewModel model);
    Task<bool> DeleteModifier(int modifierId, int modifierGroupId);

}