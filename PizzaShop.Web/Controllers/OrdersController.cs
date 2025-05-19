using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Implementations;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    public readonly IOrderService _orderService;
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [CustomAuthorize("Order", "CanView")]
    public async Task<IActionResult> Orders()
    {
        try
        {
            OrdersListViewModel viewModel = await _orderService.GetOrdersListModel();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Orders", "Orders");
        }
    }

    // GetOrderByPagination
    [HttpGet]
    public async Task<IActionResult> GetOrderByPagination(string searchTerm = "", int page = 1, int pageSize = 2, string SortBy = "Orderid", string SortOrder = "asc", string statusLog = "All Status", string timeLog = "All Time", string fromDate = "", string toDate = "")
    {
        try
        {
            OrdersListViewModel model = await _orderService.GetOrderByPaginationAsync(searchTerm, page, pageSize, SortBy, SortOrder, statusLog, timeLog, fromDate, toDate);
            return PartialView("_OrderList", model);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Orders", "Orders");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportOrdersInExcel(string searchTerm, string statusLog, string timeLog)
    {
        try
        {
            byte[] model = await _orderService.ExportDataInExcel(searchTerm, statusLog, timeLog);
            string fileName = $"Orders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
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
            return RedirectToAction("Orders", "Orders");
        }

    }

    public async Task<IActionResult> OrderDetails(int orderId)
    {
        try
        {
            OrderDetailViewModel viewModel = await _orderService.GetOrderDetail(orderId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Orders", "Orders");
        }
    }

    public async Task<IActionResult> OrderInvoice(int orderId)
    {
        try
        {
            byte[] pdfBytes = await _orderService.GenerateInvoicePdfAsync(orderId);
            return File(pdfBytes, "application/pdf", $"Invoice_{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> OrderDetailPDF(int orderId)
    {
        try
        {
            byte[] pdfBytes = await _orderService.GenerateOrderDetailPdfAsync(orderId);
            return File(pdfBytes, "application/pdf", $"OrderDetail_{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    public async Task<IActionResult> OrderDetailsPDF()
    {
        try
        {
            OrderDetailViewModel viewModel = await _orderService.GetOrderDetail(1);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return RedirectToAction("Orders", "Orders");
        }
    }
}