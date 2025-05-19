namespace PizzaShop.Entity.ViewModel;

public class SectionNameListViewModel
{
    public IEnumerable<SectionNameViewModel>? Sections {get;set;}

    public int? SelectedSection {get;set;}

}