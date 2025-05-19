using Microsoft.AspNetCore.Mvc;

namespace PizzaShop.Web.Controllers;

public class ErrorPagesController : Controller
{
    [Route("ErrorPages/ShowError/{statusCode}")]
    public IActionResult ShowError(int statusCode)
    {
        string statusHeader = statusCode switch
        {
            404 => "The page you are looking for was not found.",
            500 => "An unexpected server error occurred.",
            403 => "Unauthorized",
            401 => "Unauthenticated",
            _ => "An unexpected error occurred.",
        };

        string statusMessage = statusCode switch
        {
            404 => "Oops, the page you're looking for isn't here.",
            500 => "An unexpected server error occurred.",
            403 => "You do not have permission to access this resource.",
            401 => "You are not authenticated to access this resource.",
            _ => "An unexpected error occurred.",
        };

        ViewData["StatusCode"] = statusCode;
        ViewData["StatusHeader"] = statusHeader;
        ViewData["StatusMessage"] = statusMessage;
        return View("ErrorPage");
    }
}
