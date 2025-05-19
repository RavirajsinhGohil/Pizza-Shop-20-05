using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Interfaces;

public interface IUserRepository
{
    Task<User> CheckUserEmailAndPassword(string email, string password);
    User GetUserByEmail(string email);
    User GetUserById(int id);

    IQueryable<User> GetUsers();

    bool UpdateUser(User user);

    IQueryable<User> GetUsersQuery();
    Role GetRoleById(int id);
    void AddUser(User user);

    void DeleteUser(User user);

    Task<List<PermissionsViewModel>> GetPermissionsByRoleAsync(string roleName);

    Task<bool> UpdateRolePermissionsAsync(List<PermissionsViewModel> permissions);

}
