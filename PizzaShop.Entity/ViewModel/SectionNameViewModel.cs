namespace PizzaShop.Entity.ViewModel;

public class SectionNameViewModel
{
    public int SectionId { get; set; }
    public string SectionName { get; set; } = null!;
    public string? Description { get; set; }
    public List<TableViewModel>? Tables { get; set; } = new();
}