namespace PizzaShop.Entity.ViewModel;

public class TableViewModel
{
    public int TableId { get; set; } 
    public string? TableName { get; set; } 
    public int? SectionId { get; set; }
    public string? SectionName { get; set; } 
    public string? Name { get; set; }

    public int Capacity { get; set; }

    public bool Status { get; set; } 

}