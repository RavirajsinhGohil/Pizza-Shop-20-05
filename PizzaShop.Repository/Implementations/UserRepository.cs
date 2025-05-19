using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Implementations;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbo;
    public UserRepository(ApplicationDbContext dbo)
    {
        _dbo = dbo;
    }

    public async Task<User> CheckUserEmailAndPassword(string email, string password)
    {
        User user = await _dbo.Users.FirstAsync(u => u.Email == email);
        if(user == null)
        {
            return null;
        }

        if (BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return user;
        }
        return null;
    }

    public User GetUserByEmail(string email)
    {
        return _dbo.Users.FirstOrDefault(u => u.Email == email);
    }

    public IQueryable<User> GetUsers()
    {
        return _dbo.Users.AsQueryable();
    }


    public User GetUserById(int id)
    {
        return _dbo.Users.FirstOrDefault(u => u.Userid == id);
    }

    public bool UpdateUser(User user)
    {
        _dbo.Users.Update(user);
        _dbo.SaveChanges();
        return true;
    }

    public IQueryable<User> GetUsersQuery()
    {
        // return null;
        return _dbo.Users.Include(u => u.Role).Where(u => !u.Isdeleted);
    }

    public Role GetRoleById(int id)
    {
        return _dbo.Roles.FirstOrDefault(r => r.Roleid == id);
    }

    public void AddUser(User user)
    {
        _dbo.Users.Add(user);
         _dbo.SaveChanges();
    }

    public void DeleteUser(User user)
    {
        user.Isdeleted = true;
        _dbo.SaveChanges();
    }

    public async Task<List<PermissionsViewModel>> GetPermissionsByRoleAsync(string roleName)
    {
        var role = await _dbo.Roles.FirstOrDefaultAsync(r => r.Rolename == roleName);
        if (role == null)
        {
            return null;
        }
        var permissions = await _dbo.Permissions
            .Where(rp => rp.Roleid == role.Roleid)
            .OrderBy(rp => rp.Permissionid)
            .Select(rp => new PermissionsViewModel
            {
                RoleName = role.Rolename,
                RoleId = rp.Roleid,
                PermissionId = rp.Permissionid,
                PermissionName = rp.Permissionname,
                CanView = rp.Canview,
                CanAddEdit = rp.Canaddedit,
                CanDelete = rp.Candelete
            })
            .ToListAsync();

        return permissions;
    }

    public async Task<bool> UpdateRolePermissionsAsync(List<PermissionsViewModel> permissions)
    {
        foreach (var permission in permissions)
        {
            var rolePermission = await _dbo.Permissions
                .FirstOrDefaultAsync(rp => rp.Role.Rolename == permission.RoleName && rp.Permissionid == permission.PermissionId);

            if (rolePermission != null)
            {
                rolePermission.Canview = permission.CanView;
                rolePermission.Canaddedit = permission.CanAddEdit;
                rolePermission.Candelete = permission.CanDelete;
            }
        }

        await _dbo.SaveChangesAsync();
        return true;
    }

}
