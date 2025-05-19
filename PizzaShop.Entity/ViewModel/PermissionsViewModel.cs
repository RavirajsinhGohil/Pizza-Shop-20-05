namespace PizzaShop.Entity.ViewModel;

public class PermissionsViewModel
{
    public int PermissionId{ get;set;}
    public string? RoleName{ get;set;}
    public int RoleId{ get;set;}
    public bool AllPermissionCheckBox{ get;set;}
    public bool PermissionCheckBox{ get;set;}
    public string? PermissionName{ get;set;}

    public bool CanView{ get;set;}

    public bool CanAddEdit{ get;set;}

    public bool CanDelete{ get;set;}

}
