using System.Drawing;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using SelectPdf;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class OrderService : IOrderService
{
    public readonly IOrderRepository _orderRepository;
    public readonly IUserService _userService;
    public readonly ISectionService _sectionService;
    public readonly IMenuService _menuService;
    public readonly IRazorViewToStringRenderer _razorRenderer;
    public readonly ITaxService _taxService;
    public OrderService(IOrderRepository menuRepository, IUserService userService, ISectionService sectionService, IMenuService menuService, IRazorViewToStringRenderer razorRenderer, ITaxService taxService)
    {
        _orderRepository = menuRepository;
        _userService = userService;
        _sectionService = sectionService;
        _menuService = menuService;
        _razorRenderer = razorRenderer;
        _taxService = taxService;
    }

    public async Task<OrdersListViewModel> GetOrdersListModel()
    {
        OrdersListViewModel orderList = new OrdersListViewModel();
        orderList.orders = await _orderRepository.GetOrdersListModel();
        return orderList;
    }

    public async Task<OrdersListViewModel> GetOrderByPaginationAsync(string searchTerm, int page, int pageSize, string SortBy, string SortOrder, string statusLog, string timeLog, string fromDate, string toDate)
    {
        return await _orderRepository.GetOrderByPaginationAsync(searchTerm, page, pageSize, SortBy, SortOrder, statusLog, timeLog, fromDate, toDate);
    }

    public async Task<byte[]> ExportDataInExcel(string searchTerm, string statusLog, string timeLog)
    {
        OrdersListViewModel model = await _orderRepository.GetOrdersForExport(searchTerm, statusLog, timeLog);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage package = new())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Orders");

            Color headerColor = ColorTranslator.FromHtml("#0568A8");
            Color borderColor = Color.Black; // Black border

            // Function to Set Background Color
            void SetBackgroundColor(string cellRange, Color color)
            {
                worksheet.Cells[cellRange].Merge = true;
                worksheet.Cells[cellRange].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[cellRange].Style.Fill.BackgroundColor.SetColor(color);
                worksheet.Cells[cellRange].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[cellRange].Style.Font.Bold = true;
                worksheet.Cells[cellRange].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[cellRange].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            void SetBorder(string cellRange)
            {
                worksheet.Cells[cellRange].Merge = true;
                var border = worksheet.Cells[cellRange].Style.Border;
                border.Top.Style = ExcelBorderStyle.Thin;
                border.Bottom.Style = ExcelBorderStyle.Thin;
                border.Left.Style = ExcelBorderStyle.Thin;
                border.Right.Style = ExcelBorderStyle.Thin;
                border.Top.Color.SetColor(borderColor);
                border.Bottom.Color.SetColor(borderColor);
                border.Left.Color.SetColor(borderColor);
                border.Right.Color.SetColor(borderColor);
            }

            SetBackgroundColor("A2:B3", headerColor);
            SetBorder("C2:F3");

            SetBackgroundColor("H2:I3", headerColor);
            SetBorder("J2:M3");

            SetBackgroundColor("A5:B6", headerColor);
            SetBorder("C5:F6");

            SetBackgroundColor("H5:I6", headerColor);
            SetBorder("J5:M6");

            SetBackgroundColor("A9", headerColor);
            SetBackgroundColor("B9:D9", headerColor);
            SetBackgroundColor("E9:G9", headerColor);
            SetBackgroundColor("H9:J9", headerColor);
            SetBackgroundColor("K9:L9", headerColor);
            SetBackgroundColor("M9:N9", headerColor);
            SetBackgroundColor("O9:P9", headerColor);

            worksheet.Cells["A9"].Value = "ID";
            worksheet.Cells["A9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["B9"].Value = "Date";
            worksheet.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["E9"].Value = "Customer";
            worksheet.Cells["E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["H9"].Value = "Status";
            worksheet.Cells["H9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["K9"].Value = "Payment Mode";
            worksheet.Cells["K9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["M9"].Value = "Rating";
            worksheet.Cells["M9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["O9"].Value = "Total Amount";
            worksheet.Cells["O9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            worksheet.Cells["A2:B3"].Value = "Status:";
            worksheet.Cells["C2:F3"].Value = string.IsNullOrEmpty(statusLog) || statusLog == "All Status" ? "All Status" : statusLog;
            worksheet.Cells["C2:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C2:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["H2:I3"].Value = "Date:";
            worksheet.Cells["J2:M3"].Value = string.IsNullOrEmpty(timeLog) || timeLog == "AllTime" ? "AllTime" : timeLog;
            worksheet.Cells["J2:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J2:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["A5:B6"].Value = "Search Text:";
            worksheet.Cells["C5:F6"].Value = string.IsNullOrEmpty(searchTerm) ? "" : searchTerm;
            worksheet.Cells["C5:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C5:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["H5:I6"].Value = "No. Of Records:";
            worksheet.Cells["J5:M6"].Value = model.orders.Count;
            worksheet.Cells["J5:M6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J5:M6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logos", "pizzashop_logo.png");
            if (File.Exists(logoPath))
            {
                var logo = worksheet.Drawings.AddPicture("Logo", new FileInfo(logoPath));
                logo.SetPosition(1, 0, 14, 0);
                logo.SetSize(100, 100);
            }

            int row = 10;
            foreach (var order in model.orders)
            {
                worksheet.Cells[$"A{row}"].Value = order.Orderid;
                worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"B{row}:D{row}"].Merge = true;
                worksheet.Cells[$"B{row}"].Value = order.Createdat.ToString();
                worksheet.Cells[$"B{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"E{row}:G{row}"].Merge = true;
                worksheet.Cells[$"E{row}"].Value = order.CustomerName;
                worksheet.Cells[$"E{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"E{row}:G{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"E{row}:G{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"H{row}:J{row}"].Merge = true;
                worksheet.Cells[$"H{row}"].Value = order.Status;
                worksheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{row}:J{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"H{row}:J{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"K{row}:L{row}"].Merge = true;
                worksheet.Cells[$"K{row}"].Value = order.PaymentMode;
                worksheet.Cells[$"K{row}:L{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"K{row}:L{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"K{row}:L{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"M{row}:N{row}"].Merge = true;
                worksheet.Cells[$"M{row}"].Value = order.Rating ?? 0;
                worksheet.Cells[$"M{row}:N{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"M{row}:N{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"M{row}:N{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"O{row}:P{row}"].Merge = true;
                worksheet.Cells[$"O{row}"].Value = order.TotalAmount;
                worksheet.Cells[$"O{row}:P{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                row++;
            }
            worksheet.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }

    public async Task<OrderDetailViewModel> GetOrderDetail(int orderId)
    {
        OrderDetailViewModel model = new();

        OrdersViewModel? order = await _orderRepository.GetOrderById(orderId);
        CustomerViewModel customer = await _orderRepository.GetCustomerById(order.Customerid);

        model.order = await _orderRepository.GetOrderById(orderId);
        model.customer = await _orderRepository.GetCustomerById(order.Customerid);
        model.tables = await _orderRepository.GetTablesByOrderId(order.Orderid);
        model.items = await _orderRepository.GetItemsByOrderId(orderId);
        model.Taxes = await _taxService.GetTaxesByOrderId(orderId);
        model.order.TotalAmount = model.Taxes.Sum(ta => ta.TaxAmount) ?? 0;

        // model.order.SubTotal = model.items.Sum(i => i.OrderTotalAmount) + model.modifiers.Sum(i => i.OrderTotalAmount) ?? 0;

        return model;
    }


    public async Task<byte[]> GenerateInvoicePdfAsync(int orderId)
    {
        OrderDetailViewModel? model = await GetOrderDetail(orderId);

        if (model.items.Count == 0)
            throw new Exception("Order not found!");

        string htmlContent = await _razorRenderer.RenderViewToStringAsync("Orders/OrderInvoice", model);
        // string htmlContent2 = await _razorRenderer.RenderViewToStringAsync("Orders/OrderDetails", model);

        var converter = new HtmlToPdf();

        converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;

        PdfDocument doc = converter.ConvertHtmlString(htmlContent);

        byte[] pdfBytes;
        using (var memoryStream = new MemoryStream())
        {
            doc.Save(memoryStream);
            pdfBytes = memoryStream.ToArray();
        }

        doc.Close();

        return pdfBytes;
    }

    public async Task<byte[]> GenerateOrderDetailPdfAsync(int orderId)
    {
        OrderDetailViewModel? model = await GetOrderDetail(orderId) ?? throw new Exception("Order not found!");

        string htmlContent = await _razorRenderer.RenderViewToStringAsync("Orders/OrderInvoice", model);
        string cssContent = System.IO.File.ReadAllText("wwwroot/css/style.css");
        htmlContent = $"<style>{cssContent}</style>" + htmlContent;
        // string htmlContent = await _razorRenderer.RenderViewToStringAsync(link, model);

        HtmlToPdf? converter = new HtmlToPdf();

        converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;

        PdfDocument doc = converter.ConvertHtmlString(htmlContent);

        byte[] pdfBytes;
        using (var memoryStream = new MemoryStream())
        {
            doc.Save(memoryStream);
            pdfBytes = memoryStream.ToArray();
        }

        doc.Close();

        return pdfBytes;
    }



}