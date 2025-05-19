using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class CustomAuthorize : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _moduleName;
    private readonly string _permissionType;

    public CustomAuthorize(string moduleName, string permissionType)
    {
        _moduleName = moduleName;
        _permissionType = permissionType;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        ClaimsPrincipal? user = context.HttpContext.User;

        if(!user.Identity.IsAuthenticated)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string? userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        string? email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if(string.IsNullOrEmpty(userRole))
        {
            context.Result = new RedirectToRouteResult(new { Controller = "ErrorPages", action = "ShowError", statusCode = "403" });
            return;
        }

        IUserService? permissionService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        if (permissionService == null)
        {
            context.Result = new RedirectToRouteResult(new { Controller = "ErrorPages", action = "ShowError", statusCode = "403" });
            return;
        }

        List<Entity.ViewModel.PermissionsViewModel>? permissions = await permissionService.GetPermissionsByRoleAsync(userRole);

        bool hasPermission = permissions.Any(p =>
            p.PermissionName == _moduleName && 
            ((_permissionType == "CanView" && p.CanView) ||
            (_permissionType == "CanAddEdit" && p.CanAddEdit) ||
            (_permissionType == "CanDelete" && p.CanDelete)));

        Console.WriteLine($"User Role: {userRole}, Module: {_moduleName}, Permission Type: {_permissionType}, Has Permission: {hasPermission}");

        if (!hasPermission)
        {
            Console.WriteLine("Access Denied!");
            context.Result = new RedirectToRouteResult(new { Controller = "ErrorPages", action = "ShowError", statusCode = "403" });
        }
        else
        {
            Console.WriteLine("Access Granted!");
        }
    }

}
