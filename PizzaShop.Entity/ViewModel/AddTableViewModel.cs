using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModel;

public class AddTableViewmodel
{
    public int? TableId { get; set; }

    [Required(ErrorMessage = "Please select Section")]
    public int SectionId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; } 

    
    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
    public int? Capacity { get; set; }
    public string Status { get; set; } = null!;
}