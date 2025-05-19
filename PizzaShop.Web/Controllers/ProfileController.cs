using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

public class ProfileController : Controller
{
    private readonly IUserService _userService;
    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Profile()
    {
        string? token = Request.Cookies["Token"];
        string? email = _userService.GetEmailFromToken(token);

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Login");
        }

        UserViewModel? model = _userService.GetUserProfile(email);

        if (model == null)
        {
            return NotFound("User Not Found");
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Profile(UserViewModel model)
    {
        string? token = Request.Cookies["Token"];
        model.Email = _userService.GetEmailFromToken(token);

        if (string.IsNullOrEmpty(model.Email))
        {
            return RedirectToAction("Login", "Login");
        }
        if(ModelState.IsValid)
        {
            bool success = _userService.UpdateUserProfile(model.Email, model);

            if (!success)
            {
                return NotFound("User Not Found");
            }

            TempData["success"] = "Profile Updated Successfully.";
            return View(model);
        }
        else
        {
            return View();
        }

        
    }

    [HttpGet]
    public IActionResult ProfileChangePassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ProfileChangePassword(ProfileChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            string? token = Request.Cookies["Token"];
            string? userEmail = _userService.GetEmailFromToken(token);
            model.Email = userEmail;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Login");
            }

            string? result = _userService.ChangePassword(userEmail, model);

            if (result == "UserNotFound")
            {
                TempData["error"] = "User not found.";
                return View(model);
            }

            if (result == "IncorrectPassword")
            {
                TempData["error"] = "Incorrect Current Password.";
                // TempData["error"] = "Current Password is incorrect.";
                return View(model);
            }

            TempData["success"] = "Password Updated Successfully.";
            return RedirectToAction("Login", "Login");
        }
        return View();

    }
}
