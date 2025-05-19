using PizzaShop.Entity.Models;

namespace PizzaShop.Entity.ViewModel;

public class CategoryItemsForOrderMenuViewModel
{
    public List<Menucategory>? Categories { get; set; }
    public List<Item>? Items { get; set; }
    public int? SelectedCategoryId { get; set; }
    public int? ActiveOrderId { get; set; }
    public string? SectionName { get; set; }
    public List<string>? TableName { get; set; }
    public List<Taxesandfee> Taxes { get; set; } = new();

}
