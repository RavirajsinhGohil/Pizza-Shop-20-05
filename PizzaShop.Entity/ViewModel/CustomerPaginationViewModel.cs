namespace PizzaShop.Entity.ViewModel;

public class CustomerPaginationViewModel
{
    public string SearchTerm { get; set; } = "";
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public string? SortBy { get; set; } = "CustomerId";
    public string? SortOrder { get; set; } = "asc";
    public string? TimeLog { get; set; } ="All Time";
    public string? CustomFromDate { get; set; }
    public string? CustomToDate { get; set; }
    public int? FromRec { get; set; }
    public int? ToRec { get; set; }

}
