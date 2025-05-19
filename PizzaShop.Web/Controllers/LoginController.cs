using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;
using static PizzaShop.Service.Implementations.UserService;
using PizzaShop.Entity.Constants;
using PizzaShop.Entity.Models;
using System.Text.Json;

namespace PizzaShop.Web.Controllers;

public class LoginController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;
    public LoginController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        string? email = Request.Cookies["Email"];
        if (!string.IsNullOrEmpty(email))
        {
            int roleId = int.Parse(Request.Cookies["RoleId"]);
            string? roleName = _userService.GetRoleNameById(roleId);
            if (roleName != null)
            {
                var userPermissions = _userService.GetPermissionsByRoleAsync(roleName).Result;
                HttpContext.Session.SetString("Permissions", JsonSerializer.Serialize(userPermissions));
            }

            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // var password = BCrypt.Net.BCrypt.HashPassword(model.password);
                UserViewModel user = await _authService.Login(model.Email, model.password);
                if (user == null)
                {
                    TempData["error"] = Constants.InvalidCredentials;
                    return View();
                }
                
                string token = _authService.GenerateJwtToken(user.Email, user.RoleId);

                List<PermissionsViewModel>? userPermissions = await _userService.GetPermissionsByRoleAsync(user.Rolename);

                CookieOptions? cookie = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                };

                CookieOptions? JWTtoken = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                };

                Response.Cookies.Append("Token", token, JWTtoken);
                Response.Cookies.Append("Username", user.Username, cookie);
                if(user.Profileimagepath != null)
                {
                    Response.Cookies.Append("Image", user.Profileimagepath, cookie);
                }
                
                Response.Cookies.Append("RoleId", user.RoleId.ToString(), cookie);

                if (model.RememberMe)
                {
                    Response.Cookies.Append("Email", model.Email, cookie);
                    cookie.Expires = DateTime.UtcNow.AddDays(30);
                }

                HttpContext.Session.SetString("Permissions", JsonSerializer.Serialize(userPermissions));

                TempData["success"] = Constants.LoginSuccessfull;
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Login", "Login");
        }
    }

    public IActionResult Logout()
    {
        try
        {
            foreach (string? cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return RedirectToAction("Login", "Login");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Login", "Login");
        }
    }

    [HttpGet]
    public IActionResult ForgotPassword(string? email)
    {
        try
        {
            if (!string.IsNullOrEmpty(email))
            {
                ViewData["Email"] = email;
            }
            else
            {
                ViewData["Email"] = "";
            }
            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Login", "Login");
        }
    }

    [HttpPost]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool emailExist = _authService.CheckEmailExist(model.Email);
            User user = _userService.GetUserByEmail(model.Email);
            if (emailExist)
            {
                string filePath = @"C:\Users\pct216\Downloads\Pizza Shop\Main Project\Pizza Shop\PizzaShop.Web\EmailTemplate\ResetPasswordEmailTemplate.html";
                string emailBody = System.IO.File.ReadAllText(filePath);

                string? token = _authService.GenerateJwtTokenForgot(user, false);

                string? url = Url.Action("ResetPassword", "Login", new { token = Uri.EscapeDataString(token) }, Request.Scheme);
                emailBody = emailBody.Replace("{ResetLink}", url);

                string subject = "Reset Password";
                _authService.SendEmailAsync(model.Email, subject, emailBody);

                TempData["success"] = Constants.PasswordResetLinkForwarded;
            }
            else
            {
                TempData["error"] = Constants.InvalidEmail;
                return RedirectToAction("ForgotPassword", "Login");
            }
        }
        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPassword(string token)
    {
        try
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Login", "Login");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        try
        {
            string? token = model.Token;
            if (TokenBlacklist.Contains(token))
            {
                TempData["error"] = Constants.InvalidToken;
                return RedirectToAction("ForgotPassword", "Login"); ; // Token is invalid
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, Constants.PasswordNotMaches);
                TempData["error"] = Constants.PasswordNotMaches;
                return View(model);
            }

            bool result = await _userService.ResetPassword(model.Token, model.NewPassword);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, Constants.NotResetPassword);
                TempData["error"] = Constants.NotResetPassword;
                return View(model);
            }
            TempData["success"] = Constants.ResetPassword;
            return RedirectToAction("Login", "Login");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Customers", "Customers");
        }
    }
}