using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Helper;

public static class PermissionHelper
{
    
    public static async Task<PermissionsViewModel> GetPermissionsAsync(HttpContext context, string requiredPermissionName)
    {
        ClaimsPrincipal? userClaims = context.User;
        if (userClaims?.Identity == null || !userClaims.Identity.IsAuthenticated)
            return new PermissionsViewModel();
 
        string? roleName = userClaims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        if (string.IsNullOrEmpty(roleName))
            return new PermissionsViewModel();
 
        IUserService? userService = context.RequestServices.GetService<IUserService>();
        if (userService == null)
            return new PermissionsViewModel();
 
        List<PermissionsViewModel>? permissions = await userService.GetPermissionsByRoleAsync(roleName);
 
        PermissionsViewModel? permission = permissions.FirstOrDefault(p => p.PermissionName == requiredPermissionName);
        if (permission == null)
            return new PermissionsViewModel();
 
        return new PermissionsViewModel
        {
            CanView = permission.CanView,
            CanAddEdit = permission.CanAddEdit,
            CanDelete = permission.CanDelete
        };
    }
}