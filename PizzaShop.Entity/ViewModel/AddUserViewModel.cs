using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Entity.ViewModel;

public class AddUserViewModel
{
    public int? UserId { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    [MaxLength(20, ErrorMessage = "First Name must be a maximum of 20 characters")]
    public string? Firstname { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [MaxLength(20, ErrorMessage = "Last Name must be a maximum of 20 characters")]
    public string? Lastname { get; set; }

    [MaxLength(20, ErrorMessage = "User Name must be a maximum of 20 characters")]
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email")]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[.@$!%*?&])[A-Za-z\d.@$!%*?&]{8,}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character (., @$!%*?&)")]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [MaxLength(10, ErrorMessage = "Phone must be maximum of 10 characters")]
    [RegularExpression(@"^\(?([6-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone")]
    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    [Required(ErrorMessage = "Please select Role")]
    public string? Rolename { get; set; }

    public string? Status { get; set; }

    [Required(ErrorMessage = "Please select Country")]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Please select State")]
    public string? State { get; set; }

    [Required(ErrorMessage = "Please select City")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Zipcode is required")]
    [RegularExpression(@"^(?!000000)\d{6}$", ErrorMessage = "Zipcode must be 5 or 6 digits.")]
    public string? Zipcode { get; set; }


    [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; }

    public string? Createdby { get; set; }

    public IFormFile? ProfileImage { get; set; }

    public string? ProfileImagePath { get; set; }
}