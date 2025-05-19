using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModel;

public class MenuItemViewModel
{
    [Required]
    public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    [Required]
    public List<ItemViewModel> Items { get; set; } = new List<ItemViewModel>();
    public List<ModifierGroupViewModel> ModifierGroups { get; set; } = new List<ModifierGroupViewModel>();
    public List<ModifiersViewModel> Modifiers { get; set; } = new List<ModifiersViewModel>();

    [Required]
    public CategoryViewModel Category { get; set; } = new CategoryViewModel();

    [Required]
    public ItemViewModel Item { get; set; } = new ItemViewModel();

    [Required]
    public ModifierGroupViewModel ModifierGroup { get; set; } = new ModifierGroupViewModel();
    [Required]
    public ModifiersViewModel Modifier { get; set; } = new ModifiersViewModel();
    public int? CurrentPage { get; set; }
    public int? totalItems { get; set; }
    public int? TotalPages { get; set; }
    public int? PageSize { get; set; }
    public int? FromItem { get; set; }
    public int? ToItem { get; set; }

    public int? CurrentPageModifiers { get; set; }
    public int? TotalItemsModifiers { get; set; }
    public int? TotalPagesModifiers { get; set; }
    public int? PageSizeModifiers { get; set; }
    public int? FromModifiers { get; set; }
    public int? ToModifiers { get; set; }
    public int SelectedCategoryId { get; set; }
    // public IFormFile ItemPhoto { get; set; }
    public List<int> SelectedModifiers { get; set; } = new List<int>();
    
}
