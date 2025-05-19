using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Repository.Interfaces;

namespace PizzaShop.Repository.Implementations;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _dbo;
    public DashboardRepository(ApplicationDbContext dbo)
    {
        _dbo = dbo;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(string filter)
    {
        DateTime startDate, endDate;
        DateTime today = DateTime.Today;

        switch (filter)
        {
            case "All Time":
                startDate = new DateTime(2020, 1, 1); // Assuming the start date of your data
                endDate = today.AddDays(1);
                break;
            case "Today":
                startDate = today;
                endDate = today.AddDays(1);
                break;
            case "Last 7 Days":
                startDate = today.AddDays(-6);
                endDate = today.AddDays(1);
                break;
            case "Last 30 Days":
                startDate = today.AddDays(-29);
                endDate = today.AddDays(1);
                break;
            case "Current Month":
                startDate = new DateTime(today.Year, today.Month, 1);
                endDate = startDate.AddMonths(1);
                break;
            default:
                startDate = new DateTime(today.Year, today.Month, 1);
                endDate = startDate.AddMonths(1);
                break;
        }

        List<Order>? ordersInRange = await _dbo.Orders
            .Where(o => o.Createdat >= startDate && o.Createdat < endDate)
            .ToListAsync();

        decimal totalSales = ordersInRange.Sum(o => o.Totalamount);
        int totalOrders = ordersInRange.Count;
        decimal avgOrderValue = Math.Round(totalOrders > 0 ? totalSales / totalOrders : 0,2);

        // Serving time calculation from OrderDetails
         List<Orderdetail>? servedDetails = await _dbo.Orderdetails
            .Include(od => od.Order)
            .Where(od =>
                // od.Servingtime != null &&
                od.Order != null &&
                od.Order.Createdat >= startDate &&
                od.Order.Createdat < endDate &&
                od.Order.Createdat != null)
            .ToListAsync();

        var avgWaitingTime = _dbo.Waitingtickets.Where(c => c.Tableassigntime != null && c.Createdat != null).Average(c => (c.Tableassigntime - c.Tableassigntime).Value.TotalMinutes);
        //  _dbo.Waitingtickets.Average(c => (double)(c.Tableassigntime - c.Createdat) );

        // double avgWaitingTime = servedDetails.Any()
        //     ? servedDetails.Average(od => (od.Servingtime.Value - od.Order.Createdat.Value).TotalMinutes)
        //     : 0;

        // var dailyCounts = _dbo.Customers
        //                     .Where(c => c.Createdat != null && c.Createdat >= fromDate && c.Createdat < toDate)
        //                     .GroupBy(c => c.Createdat.Value.Date)
        //                     .Select(g => new
        //                     {
        //                         Date = g.Key,
        //                         Count = g.Count()
        //                     })
        //                     .OrderBy(x => x.Date)
        //                     .ToList();
 
        // var cumulativeList = new List<ChartDataPoint>();
        // int runningTotal = 0;
        // foreach (var item in dailyCounts)
        // {
        //     runningTotal += item.Count;
        //     cumulativeList.Add(new ChartDataPoint
        //     {
        //         Label = item.Date.ToString("MMM dd"),
        //         Value = runningTotal
        //     });
        // }
 
        // Revenue chart data
        List<ChartDataPoint>? revenueChart = ordersInRange
            .GroupBy(o => o.Createdat.Date)
            .Select(g => new ChartDataPoint
            {
                Label = g.Key.ToString("MMM dd"),
                Value = g.Sum(o => o.Totalamount)
            })
            .OrderBy(g => g.Label)
            .ToList();

        // Customer growth
        List<ChartDataPoint>? customerGrowth = _dbo.Customers
            .Where(c => c.Createdat != null && c.Createdat >= startDate && c.Createdat < endDate)
            .GroupBy(c => c.Createdat.Date)
            .AsEnumerable() 
            .Select(g => new ChartDataPoint
            {
                Label = g.Key.ToString("MMM dd"),
                Value = g.Count()
            })
            .OrderBy(e => e.Label)
            .ToList(); 

        // Top selling items
        List<TopItem>? topItems = await _dbo.Orderdetails
            .Where(od => od.Order.Createdat >= startDate && od.Order.Createdat < endDate)
            .GroupBy(od => od.Item.Itemname)
            .OrderByDescending(g => g.Sum(od => od.Quantity))
            .Take(5)
            .Select(g => new TopItem
            {
                Name = g.Key,
                OrderCount = g.Sum(od => od.Quantity),
                ImageUrl = "/images/dining-menu.png"
            })
            .ToListAsync();

        // Least selling items
        List<TopItem>? leastItems = await _dbo.Orderdetails
            .Where(od => od.Order.Createdat >= startDate && od.Order.Createdat < endDate)
            .GroupBy(od => od.Item.Itemname)
            .OrderBy(g => g.Sum(od => od.Quantity))
            .Take(5)
            .Select(g => new TopItem
            {
                Name = g.Key,
                OrderCount = g.Sum(od => od.Quantity),
                ImageUrl = "/images/dining-menu.png"
            })
            .ToListAsync();

        int waitingCount = await _dbo.Waitingtickets.Where(i => i.Tableassigntime == null).CountAsync();
        int NewCustomer = await _dbo.Customers.Where(cs => cs.Createdat >= startDate && cs.Createdat < endDate).CountAsync();

        return new DashboardViewModel
        {
            TotalSales = totalSales,
            TotalOrders = totalOrders,
            AverageOrderValue = avgOrderValue,
            AverageWaitingTime = Math.Round(avgWaitingTime, 2),
            RevenueChartData = revenueChart,
            CustomerGrowthData = customerGrowth,
            TopSellingItems = topItems,
            LeastSellingItems = leastItems,
            WaitingListCount = waitingCount,
            NewCustomer = NewCustomer
        };
    }
}
