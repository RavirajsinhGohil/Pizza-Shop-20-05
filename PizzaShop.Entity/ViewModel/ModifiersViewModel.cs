using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModel;

public class ModifiersViewModel
{
    public int ModifierId { get; set; }
    public int? ModifierGroupId { get; set; }
    public string? ModifierGroupName { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Unit is required")]
    public string? Unit { get; set; }

    [Required(ErrorMessage = "Rate is required")]
    [Range(0.01, 1000, ErrorMessage = "Rate must be between 0.01 and 1000.")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, 1000, ErrorMessage = "Quantity must be between 0.01 and 10,000.")]
    public decimal? Quantity { get; set; }
    public string? Description { get; set; }
    public bool Isdeleted { get; set; }

    [Required(ErrorMessage = "Please select ModifierGroup(s)")]
    public List<int> Ids { get; set; }
    public decimal? OrderTotalAmount
    {
        get
        {
            if (OrderPrice.HasValue && OrderQuantity.HasValue)
                return OrderPrice.Value * OrderQuantity.Value;

            return null;
        }
    }
    
    // public decimal? OrderTotalAmount { get; set; } = OrderQuantity.Value * OrderPrice.Value;
    public int? OrderQuantity { get; set; }
    public decimal? OrderPrice { get; set; }
    public int? OrderDetailId { get; set; }
}
