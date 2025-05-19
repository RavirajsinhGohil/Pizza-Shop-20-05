using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Entity.ViewModel;

public class ItemViewModel
{
    public int Itemid { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Rate is required")]
    [Range(0.01, 10000, ErrorMessage = "Rate must be between 0 and 10000")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Please select Type")]
    public string? Itemtype { get; set; }

    [Required(ErrorMessage = "Please select Unit")]
    public string? Unit { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, 1000, ErrorMessage = "Quantity must be between 0 and 1000")]
    public decimal? Quantity { get; set; }
    public bool Isavailable { get; set; }
    public string? Itemimage { get; set; }
    public IFormFile? ItemPhoto { get; set; }

    [Required(ErrorMessage = "Tax Percentage is required")]
    [Range(0.01, 100, ErrorMessage = "Tax Percentage must be between 0 and 100")]
    public decimal? Tax { get; set; }
    public string? ItemShortCode { get; set; }
    public string? Description { get; set; }

    [Required(ErrorMessage = "Please select Category")]
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public bool Isdeleted { get; set; }
    public decimal? OrderTotalAmount
    {
        get
        {
            if (OrderPrice.HasValue && OrderQuantity.HasValue)
                return OrderPrice.Value * OrderQuantity.Value;

            return null;
        }
    }
    public int? OrderQuantity { get; set; }
    public decimal? OrderPrice { get; set; }
    public int? OrderDetailId { get; set; }
    public List<int?>? ModifierGroupIds { get; set; }
    public List<ModifierGorupDataViewModel>? ModifierGroupData { get; set; }
    public List<ModifierGroupViewModel>? ModifierGroups { get; set; }
    public List<ModifiersViewModel>? Modifiers { get; set; }

      public string ItemTypeIcon
        {
            get
            {
            return Itemtype switch
            {
                "Veg" => "/images/icons/Veg-icon.svg",
                "Non-Veg" => "/images/icons/non-veg-icon.svg",
                "Vegan" => "/images/icons/vegan-icon.svg",
                _ => "/images/icons/unknown-icon.svg",
            };
        }
        }
}

public class ModifierGorupDataViewModel
{
    public int? Id { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
}