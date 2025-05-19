namespace PizzaShop.Entity.ViewModel;

public class TaxListPaginationViewModel
{
    public IEnumerable<TaxViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }

}