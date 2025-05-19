using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

// [Route("OrderApp")]
public class OrderAppController : Controller
{
    private readonly IOrderAppService _orderAppService;
    private readonly IMenuService _menuService;
    public readonly IOrderService _orderService;

    public OrderAppController(IOrderAppService orderAppService, IMenuService menuService, IOrderService orderService)
    {
        _orderAppService = orderAppService;
        _menuService = menuService;
        _orderService = orderService;
    }

    #region KOT

    [Authorize]
    public async Task<IActionResult> KOTMenu()
    {
        KOTViewModel? viewModel = new()
        {
            Categories = _menuService.GetCategories().Result
        };
        return View("KOTMenu", viewModel);
    }

    public async Task<IActionResult> LoadKOTCards(int categoryId, bool inProgress)
    {
        List<KOTOrderCardViewModel>? orderDetails = await _orderAppService.GetOrderDetailByCategoryId(categoryId, inProgress);

        return PartialView("_KOTCards", orderDetails);
    }

    public async Task<IActionResult> MarkAsReady([FromBody] MarkInProgressViewModel OrderDetails)
    {
        bool result = await _orderAppService.MarkOrderAsReady(OrderDetails.OrderId, OrderDetails.OrderDetailsIds);

        if (result)
        {
            return Json(new { success = true, message = "Order marked as ready." });
        }
        else
        {
            return Json(new { success = false, message = "Failed to mark order as ready." });
        }
    }

    public async Task<IActionResult> MarkasInProgress([FromBody] MarkInProgressViewModel request)
    {
        bool result = await _orderAppService.MarkOrderAsInProgress(request.OrderId, request.OrderDetailsIds);

        if (result)
        {
            return Json(new { success = true, message = "Order marked as in progress." });
        }
        else
        {
            return Json(new { success = false, message = "Failed to mark order as in progress." });
        }
    }

    #endregion

    #region Tables

    [Authorize]
    public async Task<IActionResult> TableMenu()
    {
        OrderAppTableModuleViewModel model = await _orderAppService.GetTabelModuleData();
        return View("TableMenu", model);
    }

    [HttpPost]
    public async Task<IActionResult> AddWaitingToken(OrderAppTableModuleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid data.";
            return RedirectToAction("TableMenu");
        }

        bool success = await _orderAppService.AddWaitingToken(model.WaitingToken);

        if (!success)
            return StatusCode(500, "Failed to save waiting token.");

        return RedirectToAction("TableMenu");
    }

    [HttpGet]
    public async Task<List<WaitingCustomerViewModel>?> GetWaitingCustomers(int sectionId)
    {
        List<WaitingCustomerViewModel>? waitingCustomers = await _orderAppService.GetWaitingCustomers(sectionId);

        return waitingCustomers;
    }

    [HttpPost("/OrderApp/AssignCustomerToTableAsync")]
    public async Task<IActionResult> AssignCustomerToTableAsync(AssignTableViewModel model)
    {
        int? orderId = await _orderAppService.AssignCustomerToTableAsync(model);
        if (orderId == null)
        {
            TempData["ErrorMessage"] = "Failed to assign customer to table.";
            return RedirectToAction("TableMenu", "OrderApp");
        }

        // Return to the menu page with the orderId
        return RedirectToAction("MenuMenu", new { searchText = "", categoryId = 0, orderId = orderId });
    }

    #endregion

    #region Waiting List

    [Authorize]
    public async Task<IActionResult> WaitingListMenu()
    {
        WaitingListViewModel? viewModel = await _orderAppService.GetWaitingListSections();
        return View("WaitingListMenu", viewModel);
    }

    [HttpGet("/OrderApp/LoadWaitingListCards")]
    public async Task<IActionResult> LoadWaitingListCards(int sectionId)
    {
        List<WaitingTokenViewModel>? waitingTokens = await _orderAppService.GetWaitingTokensBySectionId(sectionId);

        return PartialView("_WaitingTokenPartial", waitingTokens);
    }

    [HttpPost]
    public async Task<IActionResult> AddWaitingTokenForAll(WaitingListViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid data.";
            return RedirectToAction("WaitingListMenu");
        }

        bool success = await _orderAppService.AddWaitingToken(model.WaitingToken);

        if (!success)
            return StatusCode(500, "Failed to save waiting token.");

        TempData["SuccessMessage"] = "Waiting token added successfully.";
        return RedirectToAction("WaitingListMenu");
    }

    public async Task<IActionResult> GetWaitingTokenById(int tokenId)
    {
        AddWaitingTokenForTableViewModel? waitingToken = await _orderAppService.GetWaitingTokenById(tokenId);
        return Json(new { success = true, data = waitingToken });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateWaitingToken(WaitingListViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid data.";
            return BadRequest("Invalid data.");
        }

        bool success = await _orderAppService.UpdateWaitingToken(model.WaitingToken);

        if (!success)
            return StatusCode(500, "Failed to update waiting token.");

        TempData["SuccessMessage"] = "Waiting token updated successfully.";
        return RedirectToAction("WaitingListMenu");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteWaitingToken(int tokenId)
    {
        bool success = await _orderAppService.DeleteWaitingToken(tokenId);

        if (!success)
            return StatusCode(500, "Failed to delete waiting token.");

        TempData["SuccessMessage"] = "Waiting token deleted successfully.";
        return RedirectToAction("WaitingListMenu");
    }

    [HttpPost]
    public async Task<IActionResult> AssignTableToCustomer([FromBody] AssignTableViewModel model)
    {
        if (model == null)
        {
            return BadRequest("Invalid data.");
        }
        try
        {
            int? orderId = await _orderAppService.AssignCustomerToTableAsync(model);
            if (orderId == 0)
            {
                return BadRequest("Failed to assign tables.");
            }
            else if (orderId == -1)
            {
                return BadRequest("Insufficient capacity for the number of persons.");
            }
            return Ok(new { OrderId = orderId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public JsonResult GetSections()
    {
        var Sections = _orderAppService.GetSections();
        return Json(Sections);
    }

    [HttpGet]
    public JsonResult GetTablesBySection(int sectionId)
    {
        var tables = _orderAppService.GetTables(sectionId);

        return Json(tables);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomerByEmail(string? email)
    {
        CustomerViewModel? customer = await _orderAppService.GetCustomerByEmail(email);
        return Json(customer);
    }

    #endregion

    #region Menu

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> MenuMenu(string? searchText, int categoryId = 0, int? orderId = null)
    {
        CategoryItemsForOrderMenuViewModel? model = await _orderAppService.GetCategoryItemsForOrderMenu(searchText, categoryId, orderId);

        // Ajax item‑grid refresh
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_ItemCardsPartial", model.Items);

        return View(model);
    }

    [HttpPost]
    public IActionResult ToggleFavorite(int itemId)
    {
        _orderAppService.ToggleFavorite(itemId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetItemModifiers(int itemId)
    {
        ModifierSelectionModalViewModel? groupedModifiers = await _orderAppService.GetModifiersGroupedByItemAsync(itemId);
        return PartialView("_ItemModifiersModal", groupedModifiers);
    }

    [HttpPost]
    public async Task<IActionResult> RenderOrderItemRow([FromBody] RenderOrderItemRowRequest request)
    {
        OrderItemForRowViewModel? model = await _orderAppService.RenderOrderItemRow(request);
        return PartialView("_OrderItemRow", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderCustomer(int orderId)
    {
        OrderCustomerViewModel? data = await _orderAppService.GetOrderCustomerAsync(orderId);
        return Json(data);
    }

    [HttpPost("/OrderApp/UpdateCustomer")]
    public async Task<IActionResult> UpdateCustomer(UpdateCustomerViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
        }

        bool ok = await _orderAppService.UpdateCustomerAsync(model);
        if (!ok) return NotFound("Customer or Order not found.");

        return Ok();
    }

    [HttpGet("/OrderApp/GetAdminComment")]
    public async Task<IActionResult> GetAdminComment(int id)
    {
        string? comment = await _orderAppService.GetAdminCommentAsync(id);
        // if (comment == null)
        //     return NotFound();

        return Json(new { adminComment = comment });
    }

    [HttpPost("/OrderApp/SaveAdminComment")]
    public async Task<IActionResult> SaveAdminComment(int orderDetailId, string adminComment)
    {
        bool success = await _orderAppService.SaveAdminCommentAsync(orderDetailId, adminComment);
        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetItemInstruction(int orderId, int itemId)
    {
        string? instruction = await _orderAppService.GetItemInstructionAsync(orderId, itemId);
        instruction ??= "";

        return Json(new { instruction });
    }

    [HttpPost("/OrderApp/SaveItemInstruction")]
    public async Task<IActionResult> SaveItemInstruction(int orderId, int itemId, string instruction)
    {
        if (string.IsNullOrWhiteSpace(instruction))
            return BadRequest("Instruction is required.");

        bool success = await _orderAppService.SaveItemInstructionAsync(orderId, itemId, instruction);
        if (!success)
            return NotFound();

        return Ok();
    }

    [HttpGet("/OrderApp/GetOrderDetailsById/{orderId}")]
    public async Task<IActionResult> GetOrderDetailsById(int orderId)
    {
        try
        {
            List<SaveOrderDetailViewModel>? orderItems = await _orderAppService.GetOrderDetailsById(orderId);
            return Ok(orderItems);
        }
        catch (Exception)
        {
            return StatusCode(500, "Failed to fetch order details.");
        }
    }

    [HttpPost("/OrderApp/SaveOrderItemsAsync")]
    public async Task<IActionResult> SaveOrderItemsAsync([FromBody] SaveOrderRequestViewModel model)
    {
        if (model == null || model.OrderItems == null || !model.OrderItems.Any())
        {
            return BadRequest("Invalid order data.");
        }

        try
        {
            List<SaveOrderDetailViewModel>? orderDetailDtos = await _orderAppService.SaveOrderItemsAsync(model);
            return Ok(orderDetailDtos);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CompleteOrder(int orderId)
    {
        List<SaveOrderDetailViewModel>? orderDetailDtos = await _orderAppService.CompleteOrderAsync(orderId);
        if (orderDetailDtos == null)
        {
            return Json(new { success = false, message = "All items must be served before completing the orders." });
        }
        return Json(new { success = true, message = "Order completed successfully." });
    }

    [HttpPost]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        List<SaveOrderDetailViewModel>? orderDetailDtos = await _orderAppService.CancelOrderAsync(orderId);
        if (orderDetailDtos == null)
        {
            return Json(new { success = false, message = "Order is already served, cannot cancel." });
        }
        return Json(new { success = true, message = "Order canceled successfully." });
    }

    #endregion

}