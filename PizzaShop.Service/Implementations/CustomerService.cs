using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PizzaShop.Repository.Implementations;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class CustomerService : ICustomerService
{

    private readonly ICustomerRepository _customerRepository;

    public CustomerService (ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomersListViewModel> GetCustomersListModel()
    {
        CustomersListViewModel customerList = new CustomersListViewModel();
        customerList.Customers = await _customerRepository.GetCustomersListModel();
        return customerList;
    }

    public async Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model)
    {
        return await _customerRepository.GetCutomerByPaginationAsync(model);
    }

    public async Task<byte[]> ExportDataInExcel (CustomerPaginationViewModel viewModel)
    {
        CustomersListViewModel model = await _customerRepository.GetCustomersForExport(viewModel);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using (ExcelPackage package = new())
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Customers");

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
            worksheet.Cells["B9"].Value = "Name";
            worksheet.Cells["B9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["E9"].Value = "Email";
            worksheet.Cells["E9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["I9"].Value = "Date";
            worksheet.Cells["I9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["L9"].Value = "Mobile Number";
            worksheet.Cells["L9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            worksheet.Cells["O9"].Value = "Total Order";
            worksheet.Cells["O9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
            // worksheet.Cells["O9"].Value = "Total Amount";
            // worksheet.Cells["O9"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

            worksheet.Cells["A2:B3"].Value = "Account:";
            worksheet.Cells["C2:F3"].Value = string.IsNullOrEmpty(viewModel.TimeLog) || viewModel.TimeLog == "All Status" ? "All Status" : viewModel.TimeLog;
            worksheet.Cells["C2:F3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C2:F3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["H2:I3"].Value = "Date:";
            worksheet.Cells["J2:M3"].Value = string.IsNullOrEmpty(viewModel.TimeLog) || viewModel.TimeLog == "AllTime" ? "AllTime" : viewModel.TimeLog;
            worksheet.Cells["J2:M3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["J2:M3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["A5:B6"].Value = "Search Text:";
            worksheet.Cells["C5:F6"].Value = string.IsNullOrEmpty(viewModel.SearchTerm) ? "" : viewModel.SearchTerm;
            worksheet.Cells["C5:F6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["C5:F6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.Cells["H5:I6"].Value = "No. Of Records:";
            worksheet.Cells["J5:M6"].Value = model.Customers.Count;
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
            foreach (var customer in model.Customers)
            {

                worksheet.Cells[$"A{row}"].Value = customer.Customerid;
                worksheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"A{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"B{row}:D{row}"].Merge = true;
                worksheet.Cells[$"B{row}"].Value = customer.Firstname;
                worksheet.Cells[$"B{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"B{row}:D{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"E{row}:G{row}"].Merge = true;
                worksheet.Cells[$"E{row}"].Value = customer.Email;
                worksheet.Cells[$"E{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"E{row}:G{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"E{row}:G{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"H{row}:J{row}"].Merge = true;
                worksheet.Cells[$"H{row}"].Value = customer.CreatedAt;
                worksheet.Cells[$"H{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"H{row}:J{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"H{row}:J{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"K{row}:L{row}"].Merge = true;
                worksheet.Cells[$"K{row}"].Value = customer.Phone;
                worksheet.Cells[$"K{row}:L{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"K{row}:L{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"K{row}:L{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"M{row}:N{row}"].Merge = true;
                worksheet.Cells[$"M{row}"].Value = customer.TotalOrders;
                worksheet.Cells[$"M{row}:N{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"M{row}:N{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"M{row}:N{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                worksheet.Cells[$"O{row}:P{row}"].Merge = true;
                worksheet.Cells[$"O{row}"].Value = customer.TotalOrders;
                worksheet.Cells[$"O{row}:P{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[$"O{row}:P{row}"].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                row++;
            }

            worksheet.Cells.AutoFitColumns();

            return package.GetAsByteArray();
        }
    }

    public async Task<CustomerViewModel> GetCustomerHistoryByCustomerId(int customerId)
    {
        return await _customerRepository.GetCustomerHistoryByCustomerId(customerId);
    }

}
