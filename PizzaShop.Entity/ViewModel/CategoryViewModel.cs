using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModel;

public class CategoryViewModel
{
    public int Categoryid { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, ErrorMessage = "Name cannot be longer than 30 characters")]
    public string? Name { get; set; }
    public string? Description { get; set; }
}