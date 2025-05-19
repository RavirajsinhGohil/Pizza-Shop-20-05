using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Implementations;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    public readonly ICustomerService _customerService;
    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [CustomAuthorize("Customers", "CanView")]
    public async Task<IActionResult> Customers()
    {
        try
        {
            CustomersListViewModel viewModel = await _customerService.GetCustomersListModel();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return View();
        }
    }

    // GetCustomerByPagination
    [HttpGet]
    public async Task<IActionResult> GetCustomerByPagination(CustomerPaginationViewModel viewModel)
    {
        try
        {
            CustomersListViewModel model = await _customerService.GetCutomerByPaginationAsync(viewModel);
            return PartialView("_CustomerList", model);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Customers", "Customers");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportCustomersInExcel(string searchTerm, string timeLog)
    {
        try
        {
            CustomerPaginationViewModel viewModel = new CustomerPaginationViewModel
            {
                SearchTerm = searchTerm,
                TimeLog = timeLog
            };

            byte[] model = await _customerService.ExportDataInExcel(viewModel);
            string fileName = $"Customers_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            if (model == null || model.Length == 0)
            {
                return Json("Failed to generate Excel file.");
            }

            return File(model,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Json(ex);
        }
    }

    [HttpGet]
    public async Task<CustomerViewModel> CustomerHistory (int customerId)
    {
        CustomerViewModel customer = await _customerService.GetCustomerHistoryByCustomerId(customerId);
        return customer;
    }
}
