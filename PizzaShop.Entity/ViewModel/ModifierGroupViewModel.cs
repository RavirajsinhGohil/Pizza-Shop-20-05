using System.ComponentModel.DataAnnotations;
using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModel;

public class ModifierGroupViewModel
{
    public int ModifierGroupId { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public string? modifierGroupName { get; set; }
    public string? modifierGroupDescription { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public List<ModifiersViewModel>? ExistingModifiers { get; set; }
    public List<int>? ModifiersIds { get; set; }
}
