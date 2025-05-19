using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModel;

public class AddTaxViewModel
{
    public int? TaxId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string TaxName { get; set; } = null!;

    [Required(ErrorMessage = "Please select Type")]
    public string? Type { get; set; }

    [Required(ErrorMessage = "Tax Amount is required")]
    [Range(0, 100, ErrorMessage = "Tax Amount must be between 0 and 100")]
    public decimal TaxAmount { get; set; }

    public bool Isenable { get; set; }

    public bool Isdefault { get; set; }

}