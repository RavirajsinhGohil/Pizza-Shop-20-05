namespace PizzaShop.Entity.ViewModel;

public class TableListPaginationViewModel
{
    public int Sectionid {get;set;}
    public IEnumerable<TableViewModel>? Items { get; set; }
    public Pagination? Page { get; set; }

}