using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.Constants;
using PizzaShop.Service.Implementations;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using PizzaShop.Service.Helper;


namespace PizzaShop.Web.Controllers;

[Authorize]
public class MenuController : Controller
{
    private readonly IAuthService _authService;
    private readonly IMenuService _menuService;

    public MenuController(IAuthService authService, IMenuService menuService)
    {
        _authService = authService;
        _menuService = menuService;
    }

    [CustomAuthorize("Menu", "CanView")]
    [HttpGet("Menu")]
    public async Task<IActionResult> Menu(int categoryid = 1)
    {
        try
        {
            PermissionsViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
            ViewData["CanAddEdit"] = permission.CanAddEdit;
            ViewData["CanView"] = permission.CanView;
            ViewData["CanDelete"] = permission.CanDelete;

            MenuItemViewModel viewModel = await _menuService.GetMenuModel(categoryid);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
            // return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetItemsByCategory(int categoryid, string searchTerm = "", int page = 1, int pageSize = 2)
    {
        try
        {
            PermissionsViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
            ViewData["CanAddEdit"] = permission.CanAddEdit;
            ViewData["CanView"] = permission.CanView;
            ViewData["CanDelete"] = permission.CanDelete;

            MenuItemViewModel model = await _menuService.GetItemsByCategoryAsync(categoryid, searchTerm, page, pageSize);
            return PartialView("_MenuItems", model);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory([FromBody] CategoryViewModel model)
    {
        try
        {
            bool isCategoryAdded = await _menuService.AddCategory(model);
            if (isCategoryAdded == true)
            {
                return Json(new { success = true, message = Constants.CategoryAdded });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex });
        }
    }

    [HttpGet]
    public IActionResult EditCategory(int id)
    {
        Menucategory? category = _menuService.GetCategoryForEdit(id);
        if (category == null)
        {
            return Json(new { success = false });
        }
        return Json(new { success = true, data = category });
    }

    [HttpPost]
    public async Task<IActionResult> EditCategory([FromBody] CategoryViewModel model)
    {
        try
        {
            Menucategory? category = _menuService.GetCategoryForEdit(model.Categoryid);
            if (category == null)
            {
                TempData["error"] = Constants.CategoryAdded;
                return Json(new { success = false });
            }

            bool isUpdated = await _menuService.EditCategory(model);
            if (isUpdated)
            {
                TempData["success"] = Constants.CategoryUpdated;
                return Json(new { success = true, message = Constants.CategoryUpdated });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = ex;
            return Json(new { success = false });
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteCategory(int categoryId)
    {
        try
        {
            bool IsDeleted = await _menuService.DeleteCategory(categoryId);
            if (IsDeleted == true)
            {
                TempData["success"] = Constants.CategoryDeleted;
                return RedirectToAction("Menu");
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierGroupForNewItem(int modifierGroupId)
    {
        try
        {
            ModifierGroupViewModel modifiergroup = await _menuService.GetModifierGroupForEdit(modifierGroupId);
            return Json(new { success = true, data = modifiergroup });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierGroupForEditItem(int modifierGroupId)
    {
        try
        {
            ModifierGroupViewModel modifiergroup = await _menuService.GetModifierGroupForEdit(modifierGroupId);
            return Json(new { success = true, data = modifiergroup });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddNewItem([FromForm] ItemViewModel model)
    {
        try
        {
            bool IsItemAdded = await _menuService.AddItem(model);
            if (IsItemAdded)
            {
                return Json(new { success = true, message = Constants.ItemAdded });
            }
            else
            {
                return Json(new { success = false, message = Constants.ErrorInAddItem });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public IActionResult GetMenuItemForEdit(int id)
    {
        try
        {
            MenuItemViewModel viewModel = _menuService.GetMenuItemForEdit(id).Result;
            return PartialView("_EditMenuItemModal", viewModel);
        }
        catch (Exception)
        {
            return RedirectToAction("Menu", "Menu");
        }
    }

    // POST: Edit Menu Item (update the item)
    [HttpPost]
    public async Task<IActionResult> EditMenuItem(int id, [FromForm] MenuItemViewModel model)
    {
        try
        {
            bool result = await _menuService.UpdatedMenuItem(model.Item.Itemid, model);
            if (result == false)
            {
                return Json(new { errror = true, message = Constants.ErrorInUpdateItem });
            }

            return Json(new { success = true, message = Constants.ItemUpdated });

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public IActionResult DeleteMenuItem(int itemId)
    {
        try
        {
            bool isDeleted = _menuService.DeleteItem(itemId);
            if (isDeleted == true)
            {
                TempData["success"] = Constants.ItemDeleted;
                return RedirectToAction("Menu");
            }
            else
            {
                TempData["error"] = Constants.ErrorInDeleteItem;
                return RedirectToAction("Menu");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    public async Task<IActionResult> GetModifiersForExistingModifiersForAdd(int pageNumber = 1, int pageSize = 5)
    {
        MenuItemViewModel model = _menuService.GetMenuItemModelForAddExistingModifier(pageNumber, pageSize).Result;

        return PartialView("_AddExistingModifierForAdd", model);
    }

    public async Task<IActionResult> GetModifiersForExistingModifiersForEdit(int pageNumber = 1, int pageSize = 5)
    {
        MenuItemViewModel model = _menuService.GetMenuItemModelForAddExistingModifier(pageNumber, pageSize).Result;

        return PartialView("_AddExistingModifiers", model);
    }


    [HttpPost]
    public async Task<IActionResult> AddModifierGroup([FromBody] ModifierGroupViewModel model)
    {
        try
        {
            bool isModifierGroupAdded = await _menuService.AddModifierGroup(model);
            if (isModifierGroupAdded == true)
            {
                TempData["success"] = Constants.ModifierGroupAdded;
                return Json(new { success = true });
            }
            else
            {
                TempData["error"] = Constants.ErrorInAddModifierGroup;
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = ex;
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditModifierGroup(int id)
    {
        try
        {
            ModifierGroupViewModel modifierGroupViewModel = await _menuService.GetModifierGroupForEdit(id);
            return Json(new { success = true, data = modifierGroupViewModel });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public IActionResult DeleteModifierGroup(int modifierGroupId)
    {
        bool isDeleted = _menuService.DeleteModifierGroup(modifierGroupId);
        if (isDeleted == true)
        {
            TempData["success"] = Constants.ModifierGroupDeleted;
            return RedirectToAction("Menu");
        }
        else
        {
            TempData["error"] = Constants.ErrorInDeleteModifierGroup;
            return RedirectToAction("Menu");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddNewModifier([FromBody] ModifiersViewModel model)
    {
        try
        {
            bool isAdded = await _menuService.AddModifierinGroups(model);
            if (isAdded)
            {
                return Json(new { success = true, message = Constants.ModifierAdded });
            }
            else
            {
                return Json(new { success = false, message = "Error in adding modifier." });
            }
        }
        catch (Exception ex)
        {
            TempData["error"] = ex;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetModifierItemsByModifierGroup(int categoryid, string searchTerm = "", int page = 1, int pageSize = 2)
    {
        PermissionsViewModel? permission = await PermissionHelper.GetPermissionsAsync(HttpContext, "Menu");
        ViewData["CanAddEdit"] = permission.CanAddEdit;
        ViewData["CanView"] = permission.CanView;
        ViewData["CanDelete"] = permission.CanDelete;

        MenuItemViewModel model = await _menuService.GetModifierItemsByModifierGroupAsync(categoryid, searchTerm, page, pageSize);
        return PartialView("_Modifiers", model);
    }

    [HttpGet]
    public async Task<IActionResult> EditModifier(int id)
    {
        try
        {
            MenuItemViewModel model = await _menuService.GetDataForEditModifier(id);
            return PartialView("_EditModifierModal", model);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditModifier([FromForm] MenuItemViewModel model)
    {
        bool isUpdated = await _menuService.UpdateModifier(model);
        return Json(new { success = true, message = "Modifier Item updated successfully." });
    }

    [HttpPost]
    public async Task<IActionResult> AddSelectedModifiers(int modifierGroupId, string name, string description, [FromBody] List<int> selectedModifiers)
    {
        try
        {
            bool isAdded = await _menuService.addExistingModifiersForEdit(modifierGroupId, name, description, selectedModifiers);
            if (isAdded)
            {
                return Json(new { success = true, message = "Modifier Group updated successfully." });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }

    [HttpGet]
    public async Task<IActionResult> DeleteModifier(int modifierId, int modifierGroupId)
    {
        try
        {
            bool isDeleted = await _menuService.DeleteModifier(modifierId, modifierGroupId);
            if (isDeleted == true)
            {
                // TempData["success"] = Constants.ModifierDeleted;
                return RedirectToAction("Menu");
            }
            else
            {
                // TempData["error"] = Constants.ErrorInDeleteModifier;
                return RedirectToAction("Menu");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Menu", "Menu");
        }
    }
}