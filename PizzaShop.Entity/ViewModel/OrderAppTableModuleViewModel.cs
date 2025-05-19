namespace PizzaShop.Entity.ViewModel;

public class OrderAppTableModuleViewModel
{
    public List<OrderAppSectionViewModel> Sections { get; set; } = new();

    public AddWaitingTokenForTableViewModel? WaitingToken { get; set; } = new();
}

public class OrderAppSectionViewModel
{
    public int SectionId { get; set; }
    public string? SectionName { get; set; }
    public int? Available { get; set; }
    public int? Assigned { get; set; }
    public int? Running { get; set; }

    // public int? Selected { get; set; }
    public List<TableCard>? Tables { get; set; }
}
public class TableCard
{
    public int TableId { get; set; }
    public string? TableName { get; set; }
    public int Capacity { get; set; }
    public int? OrderId { get; set; }
    public string? TableStatus { get; set; }
    public DateTime? AssignedTime { get; set; }
    public string? Amount { get; set; }
    public string? TableStatusName { get; set; }
    // public DateTime?
}
