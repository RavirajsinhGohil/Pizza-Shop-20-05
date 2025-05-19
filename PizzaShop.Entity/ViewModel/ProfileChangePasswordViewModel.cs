using System.ComponentModel.DataAnnotations;

namespace PizzaShop.Entity.ViewModel;

public class ProfileChangePasswordViewModel
{
    // [Required(ErrorMessage = "Email is required")]
    // [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = "abc@example.com";
    
    [Required(ErrorMessage = "Current Password is required")]
    // [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    // [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
    //     ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (@$!%*?&)")]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "New Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[.@$!%*?&])[A-Za-z\d.@$!%*?&]{8,}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (., @$!%*?&)")]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = "Confirm New Password is required")]
    [Compare("NewPassword", ErrorMessage = "New Password and Confirm New Password does not match")]
    public string? ConfirmPassword { get; set; }
}
