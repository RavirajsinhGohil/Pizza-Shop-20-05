namespace PizzaShop.Entity.ViewModel;

public class MarkInProgressViewModel
{
    public int OrderId { get; set; }
    public List<OrderDetailItem> OrderDetailsIds { get; set; }
}

public class OrderDetailItem
{
    public int? Id { get; set; }
    public int Quantity { get; set; }
}