namespace PizzaShop.Entity.Constants;

public class Constants
{

    #region Successfull Messages

    public const string Successfully = "Successfully!";

    #endregion

    #region Login Messages

    public const string PizzaShopLogoURL = "http://localhost:5112/images/logos/pizzashop_logo.png";

    #endregion

    #region Login Messages

    public const string InvalidCredentials = "Invalid Credentials!";
    public const string LoginSuccessfull = "Login " + Successfully;
    public const string PasswordResetLinkForwarded = "Password reset link have been sent to your email!";
    public const string InvalidEmail = "Invalid Email!";
    public const string InvalidToken = "Invalid Token!";
    public const string PasswordNotMaches = "Passwords do not match!";
    public const string NotResetPassword = "Failed To Reset Password!";
    public const string ResetPassword = "Reset Password Successfully!";

    #endregion

    #region UserList Messages

    public const string NewUserCreated = "New User Created!";
    public const string UserUpdated = "User Data Updated Successfully!";

    #endregion 

    #region Roles & Permissions Messages

    public const string NotUpdatedPermissions = "Permissions Not Updated!";

    #endregion

    #region Category Messages
    
    public const string CategoryAdded = "Category added successfully.";
    public const string CategoryNotFound = "Category not found.";
    public const string CategoryUpdated = "Category updated Successfully.";
    public const string CategoryDeleted = "Category deleted Successfully.";

    #endregion

    #region Item Messages

    public const string ItemAdded = "Item added successfully!";
    public const string ErrorInAddItem = "Error in ";
    public const string ItemUpdated = "Item updated successfully!";
    public const string ErrorInUpdateItem = "Item not updated!";
    public const string ItemDeleted = "Item deleted successfully!";
    public const string ErrorInDeleteItem = "Item not deleted!";

    #endregion

    #region Modifier Group Messages

    public const string ModifierGroupAdded = "Modifier Group added successfully!";
    public const string ErrorInAddModifierGroup = "Modifier Group not added!";
    //Remaining Edit modifier
    public const string ModifierGroupDeleted = "Modifier Group deleted!";
    public const string ErrorInDeleteModifierGroup = "Modifier Group not deleted!";

    #endregion

    #region Modifier Messages

    public const string ModifierAdded = "Modifier added successfully!";

    #endregion
}