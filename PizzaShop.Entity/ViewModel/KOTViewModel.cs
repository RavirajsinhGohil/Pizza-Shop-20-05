namespace PizzaShop.Entity.ViewModel;
public class KOTViewModel
{
    public List<CategoryViewModel>? Categories { get; set; }

    public List<KOTOrderCardViewModel>? OrderCards {get; set;}
}

public class KOTCategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class KOTOrderCardViewModel
{
    public int OrderId { get; set; }
    public List<KOTOrderSectionTableViewModel> SectionTable { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public int? Categoryid { get; set;}
    public string? CategoryName { get; set;}
    public string? Status {get; set;}

    public string? OrderInstruction { get; set; }

    // public string? ItemInstruction { get; set; }
    public List<int> OrderDetailsIds { get; set; } = new();
    
    public List<KOTOrderItemViewModel> Items { get; set; } = new();
}

public class KOTOrderItemViewModel
{
    public string ItemName { get; set; } = null!;
    public int? Quantity { get; set; }

    public int? ItemId {get; set; }
    public string? ItemInstruction {get; set; }

    // public int? PreparedQuantity {get; set;}
    public KOTCategoryViewModel Category { get; set; } = new();
    public int? InProgressQuantity {get; set;}
    public List<KOTOrderModifierViewModel> Modifiers { get; set; } = new();

}

public class KOTOrderModifierViewModel
{
    public int? ModifierId { get; set; }
    public string ModifierName { get; set; } = null!;
}

public class KOTOrderSectionTableViewModel 
{
    public string TableName {get; set;} = null!;

    public string SectionName {get; set;} = null!;
}