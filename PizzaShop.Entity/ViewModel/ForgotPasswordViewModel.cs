using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModel;

public class ForgotPasswordViewModel
{   
    [Required(ErrorMessage = "Email is Required.")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string? Email { get; set; }
}
