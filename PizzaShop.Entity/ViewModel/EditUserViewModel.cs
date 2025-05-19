using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Entity.ViewModel;

public class EditUserViewModel
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    public string? Firstname { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    public string? Lastname { get; set; }

    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    // [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(com|in|net)$", ErrorMessage = "Invalid Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [MaxLength(10, ErrorMessage = "Phone must be maximum of 10 characters")]
    [RegularExpression(@"^\(?([6-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Phone")]
    // [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")] 
    public string? Phone { get; set; }

    public int RoleId { get; set; }

    [Required(ErrorMessage = "Please select Role")]
    public string? Rolename { get; set; }

    public string? Status { get; set; }

    [Required(ErrorMessage = "Please select Country")]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Please select State")]
    public string? State { get; set; }

    [Required(ErrorMessage = "Please select City")]
    public string? City { get; set; }

    // [Required(ErrorMessage = "Zip is Required")]
    // public int? Zipcode { get; set; }
    [Required(ErrorMessage = "Zipcode is required")]
    [RegularExpression(@"^(?!000000)\d{6}$", ErrorMessage = "Zipcode must be 5 or 6 digits.")]
    public string? Zipcode { get; set; }


    [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; }

    public string? Createdby { get; set; }

    public IFormFile? ProfileImage { get; set; }

    public string? ProfileImagePath { get; set; }
}
