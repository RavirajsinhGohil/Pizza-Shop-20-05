using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Entity.ViewModel;

public class UserViewModel
{
    public string? Email { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    [MaxLength(20, ErrorMessage = "First Name must be maximum of 20 characters")]
    public string? Firstname { get; set; }
    [Required(ErrorMessage = "Last Name is required")]
    [MaxLength(20, ErrorMessage = "Last Name must be maximum of 20 characters")]
    public string? Lastname { get; set; }

    [MaxLength(20, ErrorMessage = "User Name must be maximum of 20 characters")]
    [Required(ErrorMessage = "User Name is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [MinLength(10, ErrorMessage = "Phone must be at least of 10 characters")]
    [MaxLength(10, ErrorMessage = "Phone must be maximum of 10 characters")]
    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    public string? Rolename { get; set; }

    [Required(ErrorMessage = "Please select Country")]
    public string? Country { get; set; }

    [Required(ErrorMessage = "Please select State")]
    public string? State { get; set; }


    [Required(ErrorMessage = "Please select City")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Zipcode is Required.")]
    [RegularExpression(@"^(?!000000)\d{5,6}$", ErrorMessage = "Zipcode must be 5 or 6 digits.")]
    public string? Zipcode { get; set; }


    [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; }

    public string? Profileimagepath { get; set; }
    public IFormFile? ProfileImage { get; set; }
}
