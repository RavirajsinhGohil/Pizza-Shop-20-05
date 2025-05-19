using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Entity.ViewModel;

public class AssignTableViewModel
{
    public int? Id { get; set; }
    public int? CustomerId { get; set; }
    public int? OrderId { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    [Phone]
    public string? Mobile { get; set; }

    [Required]
    [Range(1, 100)]
    public int NoOfPersons { get; set; }
    public int? SectionId { get; set; }

    [Required]
    public string? SectionName { get; set; }

    public string? SelectedTableId { get; set; }

    [NotMapped]
    public List<int>? TableIds { get; set; }

    public List<WaitingCustomerViewModel>? WaitingCustomers { get; set; }
    public List<string>? AvailableSections { get; set; }
}

public class WaitingCustomerViewModel
{
    public int? Id { get; set; }
    public int? WaitingTicketId { get; set; }
    public int? OrderId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public int? NoOfPersons { get; set; }
    public string? SectionName { get; set; }
    public int? SectionId { get; set; }
}

public class AddWaitingTokenForTableViewModel
{
    public int WaitingTokenId { get; set; }
    public int? CustomerId { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Name is required")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Mobile Number is required")]
    [MaxLength(10, ErrorMessage = "Mobile Number must be maximum of 10 characters")]
    [RegularExpression(@"^\(?([6-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid Mobile Number")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "No. of Person(s) is required")]
    [Range(1, 100, ErrorMessage = "No. of Person(s) must be between 1 and 100")]
    // [Range(1, int.MaxValue, ErrorMessage = "At least one person is required.")]
    public int TotalPersons { get; set; }

    [Required(ErrorMessage = "Please select Section")]
    public int? SectionId123 { get; set; }
    public string? SectionName { get; set; }
}