using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Service.Interfaces;

public interface IAuthService
{
    Task<UserViewModel> Login(string email, string password);
    string GenerateJwtToken(string email, int? RoleId);
    string GenerateJwtTokenForgot(User user, bool rememberMe);
    Task SendEmailAsync(string email, string subject, string htmlMessage);
    bool  CheckEmailExist(string email);
}
