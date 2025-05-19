namespace PizzaShop.Entity.ViewModel;

using System.ComponentModel.DataAnnotations;

public class ModifierSelectionModalViewModel
{
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public decimal? Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TaxPercentage { get; set; }
    public List<ModifiersGroupForMenuViewModel>? ModifierGroups { get; set; }
}

public class ModifiersGroupForMenuViewModel
{
    public string? GroupName { get; set; }
    public int Min { get; set; }
    public int Max { get; set; }
    public List<ModifiersItemForMenuViewModel> Modifiers { get; set; } = new();
}

public class ModifiersItemForMenuViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public short Price { get; set; }
    public bool IsSelected { get; set; }
    public string? ItemType { get; set; }
}

public class ModifierForMenuOrderViewModel
{
    public int ModifierId { get; set; }
    public string? ModifierName { get; set; }
    public decimal Rate { get; set; }
    public string? GroupName { get; set; }
}

public class OrderItemForRowViewModel
{
    public int Index { get; set; }
    public int? OrderId { get; set; }
    public int? ItemId { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public int MaxQuantity { get; set; }
    public decimal Rate { get; set; }
    public string? Instruction { get; set; }
    public int? ReadyQuantity { get; set; }
    public int? OrderedQuantity { get; set; }
    public List<ModifierForMenuOrderViewModel> SelectedModifiers { get; set; } = new();
    public decimal ModifiersTotal => SelectedModifiers.Sum(m => m.Rate);
}

public class RenderOrderItemRowRequest
{
    public int ItemId { get; set; }
    public int OrderId { get; set; }
    public string? ItemName { get; set; }
    public decimal BasePrice { get; set; }
    public int Quantity { get; set; }
    public int MaxQuantity { get; set; }
    public int Index { get; set; }
    public string? Instruction { get; set; }
    public List<ModifierForMenuOrderViewModel> SelectedModifiers { get; set; } = new List<ModifierForMenuOrderViewModel>();
}


public class OrderCustomerViewModel
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? NoOfPersons { get; set; }
}

public class UpdateCustomerViewModel
{
    [Required] public int OrderId { get; set; }
    [Required] public int CustomerId { get; set; }

    [Required, StringLength(60, MinimumLength = 2)]
    public string Name { get; set; } = "";

    [Required, RegularExpression(@"^\d{10}$")]
    public string Phone { get; set; } = "";

    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, Range(1, 99)]
    public int NoOfPersons { get; set; }
}


public class SaveOrderRequestViewModel
{
    public int OrderId { get; set; }
    public List<OrderItemForRowViewModel>? OrderItems { get; set; }
    public List<int>? SelectedTaxIds { get; set; } 
}

//-----------------------------------------------------------------

public class OrderItemsRequest
{
    public int OrderId { get; set; }
    public List<OrderItem>? OrderItems { get; set; }
    public List<int>? SelectedTaxIds { get; set; }
}

public class OrderItem
{
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public int AvailableQuantity { get; set; }
    public decimal Rate { get; set; }
    public string? Instruction { get; set; }
    public List<Modifier>? SelectedModifiers { get; set; }
}

public class Modifier
{
    public int ModifierId { get; set; }
    public string? ModifierName { get; set; }
    public decimal Rate { get; set; }
}

// for save viewmodel --------------------------
public class SaveOrderDetailViewModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string? ItemName { get; set; }
    public int Quantity { get; set; }
    public int MaxQuantity { get; set; }
    public decimal Rate { get; set; }
    public int? AvailableQuantity { get; set; }
    public List<SaveModifierViewModel>? Modifiers { get; set; }
}

public class SaveModifierViewModel
{
    public int ModifierId  { get; set; }
    public string? ModifierName { get; set; }
    public decimal? Rate { get; set; }
}

public class DeleteOrderDetailRequest
{
    public int OrderId { get; set; }
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public string? Instruction { get; set; }
    public List<SaveModifierViewModel>? SelectedModifiers { get; set; }
}