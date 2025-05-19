using System.Threading.Tasks;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PizzaShop.Service.Implementations;

public class MenuService : IMenuService
{
    private IMenuRepository _menuRepository;
    private IUserService _userService;

    public MenuService(IMenuRepository menuRepository, IUserService userService)
    {
        _menuRepository = menuRepository;
        _userService = userService;
    }

    public async Task<List<CategoryViewModel>> GetCategories()
    {
        return await _menuRepository.GetCategories();
    }
    public async Task<CategoryViewModel> GetCategorybyId(int categoryId)
    {
        Menucategory category = await _menuRepository.GetCategorybyId(categoryId);
        CategoryViewModel model = new CategoryViewModel()
        {
            Categoryid = category.Menucategoryid,
            Name = category.Categoryname,
            Description = category.Description
        };
        return model;
    }

    public async Task<MenuItemViewModel> GetMenuModel(int categoryId)
    {
        List<CategoryViewModel> categories = await _menuRepository.GetCategories();
        List<ItemViewModel> items = await _menuRepository.GetItems(categoryId);
        List<ModifierGroupViewModel> modifierGroups = await _menuRepository.GetModifierGroups();
        List<ModifiersViewModel> modifiers = await _menuRepository.GetModifiers(categoryId);

        return new MenuItemViewModel
        {
            Categories = categories,
            Items = items,
            ModifierGroups = modifierGroups,
            Modifiers = modifiers,
            totalItems = items.Count,
            CurrentPage = 1,
            PageSize = 5,
            TotalPages = (int)Math.Ceiling((double)items.Count / 5),
            FromItem = 1,
            ToItem = Math.Min(5, items.Count),
            SelectedCategoryId = categoryId,
            CurrentPageModifiers = 1,
            TotalItemsModifiers = modifiers.Count,
            TotalPagesModifiers = (int)Math.Ceiling((double)modifiers.Count / 5),
            PageSizeModifiers = 5,
            FromModifiers = 1,
            ToModifiers = Math.Min(5, modifiers.Count)
        };
    }

    public async Task<bool> AddCategory(CategoryViewModel model)
    {
        try
        {
            Menucategory? category = new Menucategory
            {
                Menucategoryid = model.Categoryid,
                Categoryname = model.Name,
                Description = model.Description,
                Isdeleted = false
            };
            _menuRepository.AddCategory(category);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<MenuItemViewModel> GetItemsByCategoryAsync(int categoryid, string searchTerm, int page, int pageSize)
    {
        return await _menuRepository.GetItemsByCategoryAsync(categoryid, searchTerm, page, pageSize);
    }

    public async Task<MenuItemViewModel> GetModifierItemsByModifierGroupAsync(int categoryid, string searchTerm, int page, int pageSize)
    {
        return await _menuRepository.GetModifierItemsByModifierGroupAsync(categoryid, searchTerm, page, pageSize);
    }



    public Menucategory GetCategoryForEdit(int categoryId)
    {
        return _menuRepository.GetCategoryForEdit(categoryId);
    }

    public async Task<bool> EditCategory(CategoryViewModel model)
    {
        return await _menuRepository.EditCategory(model);
    }

    public async Task<bool> DeleteCategory(int categoryId)
    {
        return await _menuRepository.DeleteCategory(categoryId);
    }

    public async Task<bool> AddItem(ItemViewModel model)
    {
        Item menuItem = new()
        {
            Categoryid = model.CategoryId,
            Itemname = model.Name,
            Itemtype = model.Itemtype,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Unit = model.Unit,
            Available = model.Isavailable,
            Tax = model.Tax,
            Itemshortcode = model.ItemShortCode,
            Description = model.Description,
            // Itemimage = _userService.(model.ItemPhoto),
            Isdeleted = false,
            Createdat = DateTime.Now,
            Isfavorite = false,
        };
        await _menuRepository.AddItem(menuItem);

        int totalItems = await _menuRepository.GetTotalCountOfItems();

        foreach (var modifierGroup in model.ModifierGroupData)
        {
            Itemmodifiergroupmapping? modifierMapping = new()
            {
                Itemid = totalItems,
                Modifiergroupid = modifierGroup.Id,
                Isitemmodifiable = false,
                Minquantity = modifierGroup.Min,
                Maxquantity = modifierGroup.Max,
            };
            await _menuRepository.AddItemModifierGroupMappings(modifierMapping);
        }
        return true;
    }

    public async Task<MenuItemViewModel> GetMenuItemForEdit(int id)
    {
        Item? item = _menuRepository.GetItemById(id);
        if (item == null)
        {
            return null;
        }

        List<CategoryViewModel>? categories = _menuRepository.GetAllCategories()
                        .Select(c => new CategoryViewModel
                        {
                            Categoryid = c.Menucategoryid,
                            Name = c.Categoryname
                        }).ToList();
        List<ModifierGroupViewModel>? modifierGroups = await _menuRepository.GetModifierGroups(); //this is how I get modifierGroup

        List<Itemmodifiergroupmapping>? ItemModifierGroup = await _menuRepository.GetModifierGroupsForEditItem(id);
        // List<ModifierGorupDataViewModel> modifierGorupDatas = ItemModifierGroup.Where(mg => mg.Itemmodifiergroupmappingid == )
        List<int?>? modifierGroupIds = ItemModifierGroup.Where(i => i.Itemid == id).Select(i => i.Modifiergroupid).ToList();

        List<ModifierGroupViewModel>? modifierGroupsForEditItem = new();

        foreach (int modifierGroupId in modifierGroupIds)
        {
            if (modifierGroupId != null)
            {
                ModifierGroupViewModel? modifiergroup = modifierGroups.FirstOrDefault(mg => mg.ModifierGroupId == modifierGroupId);

                Itemmodifiergroupmapping? mapping = ItemModifierGroup.FirstOrDefault(mg => mg.Modifiergroupid == modifierGroupId && mg.Itemid == id);
                if (modifiergroup != null && mapping != null)
                {
                    modifiergroup.Min = mapping.Minquantity;
                    modifiergroup.Max = mapping.Maxquantity;
                    // modifierGroupsForEditItem.Add(modifiergroup);
                }

                modifierGroupsForEditItem.Add(modifiergroup);
            }
        }

        return new MenuItemViewModel
        {
            Item = new ItemViewModel
            {
                Itemid = item.Itemid,
                Name = item.Itemname,
                CategoryId = item.Categoryid,
                Itemtype = item.Itemtype,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Unit = item.Unit,
                Isavailable = item.Available,
                Tax = item.Tax,
                ItemShortCode = item.Itemshortcode,
                Description = item.Description,
                Itemimage = item.Itemimage,
                Isdeleted = item.Isdeleted,
                ModifierGroupIds = modifierGroupIds,
                ModifierGroups = modifierGroupsForEditItem
            },
            Categories = categories,
            ModifierGroups = modifierGroups
        };
    }

    public async Task<bool> UpdatedMenuItem(int id, MenuItemViewModel model)
    {
        Item item = _menuRepository.GetItemById(model.Item.Itemid);
        if (item == null)
        {
            return false;
        }

        ItemViewModel? updatedItem = model.Item;
        if (updatedItem != null)
        {
            item.Itemname = updatedItem.Name;
            item.Categoryid = updatedItem.CategoryId;
            item.Itemtype = updatedItem.Itemtype;
            item.Rate = updatedItem.Rate;
            item.Quantity = updatedItem.Quantity;
            item.Unit = updatedItem.Unit;
            item.Available = updatedItem.Isavailable;
            item.Tax = updatedItem.Tax;
            item.Itemshortcode = updatedItem.ItemShortCode;
            item.Description = updatedItem.Description;
            item.Itemimage = updatedItem.Itemimage;
            item.Ismodifiable = false;
        }

        if (model.Item.ItemPhoto != null)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/items");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Item.ItemPhoto.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                model.Item.ItemPhoto.CopyToAsync(fileStream);
            }

            item.Itemimagepath = "/images/items/" + uniqueFileName;
        }

        _menuRepository.UpdateItem(item);
        List<Itemmodifiergroupmapping>? itemModifierGroupMappings = await _menuRepository.GetItemModifierGroupMappingsById(id);

        if (model.Item.ModifierGroupData != null)
        {
            foreach (var modifierGroup in model.Item.ModifierGroupData)
            {
                var mapping = itemModifierGroupMappings.FirstOrDefault(m => m.Itemid == id && m.Modifiergroupid == modifierGroup.Id);
                if (mapping != null)
                {
                    mapping.Minquantity = modifierGroup.Min;
                    mapping.Maxquantity = modifierGroup.Max;
                    await _menuRepository.UpdateItemModifierGroupMappings(mapping);
                }
                else
                {
                    Itemmodifiergroupmapping? newMapping = new Itemmodifiergroupmapping
                    {
                        Itemid = id,
                        Modifiergroupid = modifierGroup.Id,
                        Isitemmodifiable = false,
                        Minquantity = modifierGroup.Min,
                        Maxquantity = modifierGroup.Max
                    };
                    await _menuRepository.AddItemModifierGroupMappings(newMapping);
                }
            }
        }

        //Code for deleting mappings that are not in the new list
        foreach (var mapping in itemModifierGroupMappings)
        {
            if (model.Item.ModifierGroupData != null)
            {
                if (!model.Item.ModifierGroupData.Any(m => m.Id == mapping.Modifiergroupid))
                {
                    await _menuRepository.DeleteItemModifierGroupMappings(mapping);
                }
            }
            else
            {
                await _menuRepository.DeleteItemModifierGroupMappings(mapping);
            }
        }

        return true;
    }

    public bool DeleteItem(int itemId)
    {
        return _menuRepository.DeleteItem(itemId);
    }

    public async Task<List<ModifiersViewModel>> GetModifiersForExistingModifiersForAdd(int pageNumber, int pageSize)
    {
        List<Item> modifiers = await _menuRepository.GetModifiersForExistingModifiersForAdd(pageNumber, pageSize);
        List<ModifiersViewModel> modifierViewModels = new();
        foreach (var modifier in modifiers)
        {
            ModifiersViewModel modifierViewModel = new ModifiersViewModel
            {
                ModifierId = modifier.Itemid,
                Name = modifier.Itemname,
                Rate = modifier.Rate,
                Quantity = modifier.Quantity,
                Unit = modifier.Unit,
                Description = modifier.Description
            };
            modifierViewModels.Add(modifierViewModel);
        }
        return modifierViewModels;
    }

    public async Task<MenuItemViewModel> GetMenuItemModelForAddExistingModifier(int pageNumber, int pageSize)
    {
        List<ModifiersViewModel> modifiers = await GetModifiersForExistingModifiersForAdd(pageNumber, pageSize);
        int totalItems = await GetTotalModifiersCount();

        MenuItemViewModel? viewModel = new()
        {
            Modifiers = modifiers,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
            totalItems = totalItems,
            FromItem = (pageNumber - 1) * pageSize + 1,
            ToItem = Math.Min(pageNumber * pageSize, totalItems)
        };
        return viewModel;
    }

    public async Task<int> GetTotalModifiersCount()
    {
        return await _menuRepository.GetTotalCountOfModifiers();
    }

    public async Task<bool> AddModifierGroup(ModifierGroupViewModel model)
    {
        try
        {
            Modifiergroup modifiergroup = new Modifiergroup
            {
                // Modifiergroupid = model.ModifierGroupId,
                Modifiername = model.modifierGroupName,
                Description = model.modifierGroupDescription,
                Isdeleted = false
            };
            await _menuRepository.AddModifierGroup(modifiergroup);

            Modifiergroup modifiergroup1 = modifiergroup;
            int modifierMappingCount = await _menuRepository.GetTotalCountOfModifierMapping();

            for (int i = 0; i < model.ModifiersIds.Count; i++)
            {
                int modifierId = model.ModifiersIds[i];
                Item item = _menuRepository.GetItemById(modifierId);
                ModifiersViewModel modifier = new ModifiersViewModel()
                {
                    ModifierId = item.Itemid,
                    ModifierGroupId = item.Categoryid,
                    Name = item.Itemname,
                    Unit = item.Unit,
                    Rate = item.Rate,
                    Quantity = item.Quantity
                };
                var modifierMapping = new Itemmodifiergroupmapping
                {
                    Itemid = modifier.ModifierId,
                    Modifiergroupid = modifierMappingCount,
                    Isitemmodifiable = true

                };

                bool isAdded = await _menuRepository.AddItemModifierGroupMappings(modifierMapping);
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<ModifierGroupViewModel> GetModifierGroupForEdit(int id)
    {
        Modifiergroup modifierGroup = _menuRepository.GetModifierGroupById(id);
        List<ModifiersViewModel> existingModifiers = await _menuRepository.GetExistingModifiersForEdit(id);
        ModifierGroupViewModel modifierGroupViewModel = new ModifierGroupViewModel
        {
            ModifierGroupId = modifierGroup.Modifiergroupid,
            modifierGroupName = modifierGroup.Modifiername,
            modifierGroupDescription = modifierGroup.Description,
            ExistingModifiers = existingModifiers
        };
        return modifierGroupViewModel;
    }

    public bool DeleteModifierGroup(int modifierGroupId)
    {
        Modifiergroup modifierGroup = _menuRepository.GetModifierGroupById(modifierGroupId);
        modifierGroup.Isdeleted = true;
        return _menuRepository.DeleteModifierGroup(modifierGroup);
    }

    public async Task<bool> AddModifierinGroups(ModifiersViewModel model)
    {
        var menuItem = new Item
        {
            // Categoryid = modifierGroupId,
            Itemname = model.Name,
            Rate = model.Rate,
            Quantity = model.Quantity,
            Unit = model.Unit,
            Description = model.Description,
            Available = true,
            Ismodifiable = true,
            Isdeleted = false,
            Createdat = DateTime.Now,
            // Createdby = 
        };
        await _menuRepository.AddItem(menuItem);

        foreach (var modifierGroupId in model.Ids)
        {


            var modifierMapping = new Itemmodifiergroupmapping
            {
                Itemid = menuItem.Itemid,
                Modifiergroupid = modifierGroupId,
                Isitemmodifiable = true
            };

            await _menuRepository.AddItemModifierGroupMappings(modifierMapping);
        }
        return true;
    }

    public async Task<bool> addExistingModifiersForEdit(int modifierGroupId, string name, string description, List<int> selectedModifiers)
    {
        Modifiergroup modifierGroup = new Modifiergroup
        {
            Modifiergroupid = modifierGroupId,
            Modifiername = name,
            Description = description
        };

        await _menuRepository.UpdateModifierGroup(modifierGroup);

        List<ModifiersViewModel>? existingModifiers = await _menuRepository.GetExistingModifiersForEdit(modifierGroupId);
        // Delete modifiers that are not in the selectedModifiers list
        foreach (var existingModifier in existingModifiers)
        {
            if (!selectedModifiers.Contains(existingModifier.ModifierId))
            {
                Itemmodifiergroupmapping? mapping = _menuRepository.GetItemModifierGroupMappingsById(existingModifier.ModifierId, modifierGroupId);
                if (mapping != null)
                {
                    await _menuRepository.DeleteItemModifierGroupMappings(mapping);
                }
            }
        }

        for (int i = 0; i < selectedModifiers.Count; i++)
        {
            var modifierId = selectedModifiers[i];
            var mapping = _menuRepository.GetItemModifierGroupMappingsById(modifierId, modifierGroupId);
            if (mapping == null)
            {
                Item item = _menuRepository.GetItemById(modifierId);
                ModifiersViewModel modifier = new ModifiersViewModel()
                {
                    ModifierId = item.Itemid,
                    ModifierGroupId = item.Categoryid,
                    Name = item.Itemname,
                    Unit = item.Unit,
                    Rate = item.Rate,
                    Quantity = item.Quantity
                };
                Itemmodifiergroupmapping? modifierMapping = new Itemmodifiergroupmapping
                {
                    Itemid = modifier.ModifierId,
                    Modifiergroupid = modifierGroupId,
                    Isitemmodifiable = true
                };

                bool isAdded = await _menuRepository.AddItemModifierGroupMappings(modifierMapping);
            }
        }
        return true;
    }

    public async Task<MenuItemViewModel> GetDataForEditModifier(int id)
    {
        Item item = _menuRepository.GetItemById(id);

        List<ModifierGroupViewModel> modifierGroups = await _menuRepository.GetModifierGroupsForEditModifier();

        MenuItemViewModel viewModel = new MenuItemViewModel
        {
            Modifier = new ModifiersViewModel
            {
                ModifierId = item.Itemid,
                Name = item.Itemname,
                ModifierGroupId = item.Categoryid,
                Rate = item.Rate,
                Quantity = item.Quantity,
                Unit = item.Unit,
                Description = item.Description,
                Isdeleted = item.Isdeleted,
            },
            ModifierGroups = modifierGroups
        };
        return viewModel;
    }

    public async Task<bool> UpdateModifier(MenuItemViewModel model)
    {

        Item item = _menuRepository.GetItemById(model.Modifier.ModifierId);
        if (item == null)
        {
            return false;
        }

        ModifiersViewModel? updatedItem = model.Modifier;
        if (updatedItem != null)
        {
            item.Itemname = updatedItem.Name;
            item.Categoryid = updatedItem.ModifierGroupId;
            // item.Itemtype = updatedItem;
            item.Rate = updatedItem.Rate;
            item.Quantity = updatedItem.Quantity;
            item.Unit = updatedItem.Unit;
            // item.Available = updatedItem.Isavailable;
            // item.Tax = updatedItem.Tax;
            // item.Itemshortcode = updatedItem.ItemShortCode;
            item.Description = updatedItem.Description;
            // item.Itemimage = updatedItem.Itemimage;
            item.Ismodifiable = true;
        }
        _menuRepository.UpdateItem(item);
        return true;
    }

    public async Task<bool> DeleteModifier(int modifierId, int modifierGroupId)
    {
        bool isDeleted = await _menuRepository.DeleteModifier(modifierId, modifierGroupId);
        if (isDeleted)
        {
            Item item = _menuRepository.GetItemById(modifierId);
            if (item != null)
            {
                item.Isdeleted = true;
                _menuRepository.UpdateItem(item);
            }
            return true;
        }
        return false;
    }
}

