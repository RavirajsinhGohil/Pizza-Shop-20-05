using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PizzaShop.Service.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _dbo;
    // private readonly AuthService _authservice;

    public UserService(IUserRepository userRepository, ApplicationDbContext dbo)
    {
        _userRepository = userRepository;
        _dbo = dbo;
        // _authservice = authservice;
    }

    public User GetUserByEmail(string email)
    {
        return _userRepository.GetUserByEmail(email);
    }

    public string? GetRoleNameById(int roleId)
    {
        var role = _userRepository.GetRoleById(roleId);
        if (role == null)
        {
            return null;
        }
        return role.Rolename;
    }

    public async Task<bool> ResetPassword(string token, string newPassword)
    {
        var email = "tatva.pct216@outlook.com";
        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
        {
            return false;
        }
        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        var updateResult = _userRepository.UpdateUser(user);

        if (updateResult)
        {
            TokenBlacklist.Add(token);
        }

        return updateResult;
    }

    public static class TokenBlacklist
    {
        private static HashSet<string> blacklistedTokens = new HashSet<string>();

        public static void Add(string token)
        {
            blacklistedTokens.Add(token);
        }

        public static bool Contains(string token)
        {
            return blacklistedTokens.Contains(token);
        }
    }

    public UserPaginationViewModel GetUsers(string searchTerm, int page, int pageSize, string sortBy, string sortOrder)
    {
        var query = _userRepository.GetUsersQuery();

        // Apply search filter
        if (!string.IsNullOrEmpty(searchTerm.Trim()))
        {
            query = query.Where(u => u.Firstname.Trim().ToLower().Contains(searchTerm.Trim().ToLower()) || u.Lastname.Trim().ToLower().Contains(searchTerm.Trim().ToLower()) && u.Isdeleted == false);
        }

        // Sorting logic
        query = sortBy switch
        {
            "Name" => sortOrder == "asc"
                ? query.OrderBy(u => u.Firstname).ThenBy(u => u.Lastname)
                : query.OrderByDescending(u => u.Firstname).ThenByDescending(u => u.Lastname),

            "Role" => sortOrder == "asc"
                ? query.OrderBy(u => u.Role.Rolename)
                : query.OrderByDescending(u => u.Role.Rolename),

            _ => query.OrderBy(u => u.Userid) 
        };

        int totalItems = query.Count();
        var users = query.Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

        return new UserPaginationViewModel
        {
            Users = users,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            SortBy = sortBy,
            SortOrder = sortOrder
        };
    }

    public async Task<bool> AddUser(AddUserViewModel model)
    {
        var users = _dbo.Users.FirstOrDefault(u => u.Email == model.Email);
        // if (users == null)
        // {
            model.RoleId = int.Parse(model.Rolename); 
            
            var role = _userRepository.GetRoleById(model.RoleId ?? 1);
            if (role == null)
            {
                return false;
            }

            Country countryname = _dbo.Countries.FirstOrDefault(c => c.Countryid.ToString() == model.Country);
            State statename = _dbo.States.FirstOrDefault(c => c.Stateid.ToString() == model.State);
            City cityname = _dbo.Cities.FirstOrDefault(c => c.Cityid.ToString() == model.City);

            model.Country = countryname?.Countryname;
            model.State = statename?.Statename;
            model.City = cityname?.Cityname;

            Role Rolename = _dbo.Roles.FirstOrDefault(r => r.Roleid == model.RoleId);

            model.Rolename = Rolename.ToString();
            
            User? user = new()
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Username = model.Username,
                Phone = model.Phone,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Roleid = model.RoleId ?? 1,
                Country = model.Country,
                States = model.State,
                City = model.City,
                Address = model.Address,
                Zipcode = model.Zipcode,
                Createdby = model.RoleId,
                Status = "Active",
                Isdeleted = false,
            };

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/users");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyToAsync(fileStream);
                }

                user.Profileimagepath = "/images/users/" + uniqueFileName;
            }

            _userRepository.AddUser(user);
            return true;
        // }
        return false;

    }

    public EditUserViewModel GetUserForEdit(int Userid)
    {
        User user = _userRepository.GetUserById(Userid);

        if (user == null) return null;
        
        Role rolename = _dbo.Roles.FirstOrDefault(r => r.Roleid == user.Roleid);

        return new EditUserViewModel
        {
            UserId = user.Userid,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Email = user.Email,
            Username = user.Username,
            Phone = user.Phone,
            RoleId = user.Roleid,
            Rolename = rolename.Rolename,
            Country = user.Country,
            State = user.States,
            City = user.City,
            Address = user.Address,
            Zipcode = user.Zipcode,
            Status = user.Status,
            ProfileImagePath = user.Profileimagepath
        };
    }

    public bool EditUser(int Userid, EditUserViewModel model)
    {
        User user = _userRepository.GetUserById(Userid);
        if (user == null) return false;

        if (model.Country == "1" || model.Country == "2" || model.Country == "3" || model.Country == "4" || model.Country == "5")
        {
            Country countryname = _dbo.Countries.FirstOrDefault(c => c.Countryid.ToString() == model.Country);
            State statename = _dbo.States.FirstOrDefault(c => c.Stateid.ToString() == model.State);
            City cityname = _dbo.Cities.FirstOrDefault(c => c.Cityid.ToString() == model.City);

            model.Country = countryname?.Countryname;
            model.State = statename?.Statename;
            model.City = cityname?.Cityname;
        }


        Role role = _dbo.Roles.FirstOrDefault(r => r.Rolename == model.Rolename);
        model.RoleId = role.Roleid;

        user.Firstname = model.Firstname;
        user.Lastname = model.Lastname;
        user.Email = model.Email;
        user.Username = model.Username;
        user.Phone = model.Phone;
        user.Status = model.Status;
        user.Roleid = model.RoleId;
        user.Status = model.Status;
        user.Country = model.Country;
        user.States = model.State;
        user.City = model.City;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;
        user.Createdat = DateTime.Now;
        user.Profileimagepath = model.ProfileImagePath;

        _userRepository.UpdateUser(user);
        return true;
    }

    public bool DeleteUser(int id)
    {
        var user = _userRepository.GetUserById(id);
        if (user == null)
        {
            return false;
        }

        _userRepository.DeleteUser(user);
        return true;
    }

    public string GetEmailFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return string.Empty;

        var handler = new JwtSecurityTokenHandler();
        var AuthToken = handler.ReadJwtToken(token);
        return AuthToken.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email).Value;
    }

    public UserViewModel? GetUserProfile(string email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        var user = _userRepository.GetUserByEmail(email);
        if (user == null)
            return null;

        var role = _dbo.Roles.FirstOrDefault(r => r.Roleid == user.Roleid);
        UserViewModel model = new UserViewModel();
        model.Firstname = user.Firstname;
        model.Lastname = user.Lastname;
        model.Username = user.Username;
        model.Email = user.Email;
        model.Rolename = role.Rolename;
        model.Rolename = user.Role.Rolename;
        model.Country = user.Country;
        model.State = user.States;
        model.City = user.City;
        model.Phone = user.Phone;
        model.Address = user.Address;
        model.Zipcode = user.Zipcode;
        model.Profileimagepath = user.Profileimagepath;
        return model;
    }

    public bool UpdateUserProfile(string email, UserViewModel model)
    {
        var user = _userRepository.GetUserByEmail(email);
        var countryname = _dbo.Countries.FirstOrDefault(c => c.Countryid.ToString() == model.Country || c.Countryname == model.Country);
        var statename = _dbo.States.FirstOrDefault(c => c.Stateid.ToString() == model.State || c.Statename == model.State);
        var cityname = _dbo.Cities.FirstOrDefault(c => c.Cityid.ToString() == model.City || c.Cityname == model.City);

        model.Country = countryname?.Countryname;
        model.State = statename?.Statename;
        model.City = cityname?.Cityname;
        if (user == null) return false;

        user.Firstname = model.Firstname;
        user.Lastname = model.Lastname;
        user.Username = model.Username;
        user.Phone = model.Phone;
        user.Country = model.Country;
        user.States = model.State;
        user.City = model.City;
        user.Address = model.Address;
        user.Zipcode = model.Zipcode;
        user.Profileimagepath = model.Profileimagepath;

        return _userRepository.UpdateUser(user);
    }

    public async Task<List<PermissionsViewModel>> GetPermissionsByRoleAsync(string roleName)
    {
        return await _userRepository.GetPermissionsByRoleAsync(roleName);
    }
    public string ChangePassword(string email, ProfileChangePasswordViewModel model)
    {
        var user = _userRepository.GetUserByEmail(email);

        if (user == null)
        {
            return "UserNotFound";
        }

        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
        {
            return "IncorrectPassword";
        }

        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        _userRepository.UpdateUser(user);

        return "Success";
    }

    public async Task<bool> UpdateRolePermissionsAsync(List<PermissionsViewModel> permissions)
    {
        return await _userRepository.UpdateRolePermissionsAsync(permissions);
    }



}
