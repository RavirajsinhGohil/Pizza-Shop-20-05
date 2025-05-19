using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModel;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string password{ get;set;}

    public bool RememberMe { get; set; }

}
