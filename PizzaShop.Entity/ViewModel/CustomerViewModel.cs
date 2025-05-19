namespace PizzaShop.Entity.ViewModel;

public class CustomerViewModel
{
    public int Customerid { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CreatedAt { get; set; }
    public int? TotalOrders { get; set; }
    public decimal? MaxOrder { get; set; }
    public decimal? AverageBill { get; set; }
    public string? ComingSince { get; set; }
    public int? Visits { get; set; }
    public List<OrdersViewModel> CustomerOrders { get; set; }
}
