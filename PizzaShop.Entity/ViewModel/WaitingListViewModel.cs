namespace PizzaShop.Entity.ViewModel;

public class WaitingListViewModel
{
    public List<SectionModuleViewModel> Sections { get; set; } = new();
    public AddWaitingTokenForTableViewModel? WaitingToken { get; set; } = new();
}

public class SectionModuleViewModel
{
    public int SectionId { get; set; }
    public string? SectionName { get; set; }
    public List<WaitingTokenViewModel>? WaitingTokenList { get; set; } = new();
}

public class WaitingTokenViewModel
{
    public int TokenId { get; set; }
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? AssignedTime { get; set; }
    // public TimeOnly? WaitingTime => (CreatedTime.HasValue && AssignedTime.HasValue)
    //                                 ? TimeOnly.FromTimeSpan(CreatedTime.Value - AssignedTime.Value)
    //                                 : null;
    
    // public DateTime? waitinfTime => (CreatedTime.HasValue && AssignedTime.HasValue)
    //                                 ? CreatedTime.Value - AssignedTime.Value
    //                                 : null;
    public string? Amount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public int? NoOfPeople { get; set; }
}