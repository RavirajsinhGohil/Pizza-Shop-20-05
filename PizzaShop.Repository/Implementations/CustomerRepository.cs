using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Implementations;

public class CustomerRepository : ICustomerRepository
{
    private readonly ApplicationDbContext _dbo;
    public CustomerRepository(ApplicationDbContext dbo)
    {
        _dbo = dbo;
    }
    public async Task<List<CustomerViewModel>> GetCustomersListModel()
    {
        return await _dbo.Customers
            .Where(c => !c.Isdeleted)
            .OrderBy(c => c.Customerid)
            .Select(c => new CustomerViewModel
            {
                Customerid = c.Customerid,
                Firstname = c.Firstname,
                Lastname = c.Lastname,
                Email = c.Email,
                Phone = c.Phone,
                CreatedAt = c.Createdat.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalOrders = _dbo.Orders.Count(o => o.Customerid == c.Customerid)
            })
            .ToListAsync();
    }

    public async Task<CustomersListViewModel> GetCutomerByPaginationAsync(CustomerPaginationViewModel model)
    {
        IQueryable<Entity.Models.Customer>? query = _dbo.Customers.Include(c => c.Orders).AsQueryable();

        if (model.SearchTerm != null)
        {
            query = query.Where(customer => customer.Firstname.ToLower().Contains(model.SearchTerm.ToLower()));
        }

        // Apply Sorting using switch case
        query = model.SortBy switch
        {
            "Name" => model.SortOrder == "asc"
                ? query.OrderBy(c => c.Customerid)
                : query.OrderByDescending(u => u.Firstname),

            "Date" => model.SortOrder == "asc"
                ? query.OrderBy(u => u.Createdat)
                : query.OrderByDescending(u => u.Createdat),

            "Total Order" => model.SortOrder == "asc"
                ? query.OrderBy(u => u.Orders.Sum(o => o.Totalamount))
                : query.OrderByDescending(u => u.Orders.Sum(o => o.Totalamount)),

            _ => query.OrderBy(u => u.Customerid)
        };

        if (model.TimeLog != null && model.TimeLog != "All Time")
        {
            DateTime now = DateTime.Now;
            DateTime startDate = now;
            switch (model.TimeLog)
            {
                case "Last 7 days":
                    startDate = now.AddDays(-7);
                    query = query.Where(o => o.Createdat >= startDate);
                    break;
                case "Last 30 days":
                    startDate = now.AddDays(-30);
                    query = query.Where(o => o.Createdat >= startDate);
                    break;
                case "Current Month":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    query = query.Where(o => o.Createdat >= startDate);
                    break;

                case "Today":
                    startDate = new DateTime(now.Year, now.Month, now.Day);
                    query = query.Where(o => o.Createdat >= startDate);
                    break;
            }
        }

        // Apply custom date filter
        if(model.TimeLog == "Custom Date")
        {
            if (model.CustomFromDate != null && model.CustomToDate != null)
            {
                var fromDate = model.CustomFromDate;
                var toDate = model.CustomToDate;
                var fromDateTime = DateTime.Parse(fromDate.ToString());
                var toDateTime = DateTime.Parse(toDate.ToString());
                query = query.Where(o => o.Createdat >= fromDateTime && o.Createdat <= toDateTime);
            }
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)model.PageSize);

        List<CustomerViewModel>? paginatedItems = await query
            .Skip((model.Page - 1) * model.PageSize)
            .Take(model.PageSize)
            .Select(customer => new CustomerViewModel
            {
                Customerid = customer.Customerid,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedAt = customer.Createdat.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalOrders = _dbo.Orders.Count(o => o.Customerid == customer.Customerid)
            })
            .ToListAsync();
        var pagination = new CustomerPaginationViewModel
        {
            SearchTerm = model.SearchTerm,
            Page = model.Page,
            PageSize = model.PageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            SortBy = model.SortBy,
            SortOrder = model.SortOrder,
            TimeLog = model.TimeLog,
            // CustomDate = model.CustomDate,
            FromRec = (model.Page - 1) * model.PageSize + 1,
            ToRec = Math.Min(model.Page * model.PageSize, totalItems)
        };

        return new CustomersListViewModel
        {
            Customers = paginatedItems,
            Pagination = pagination
        };
    }

    public async Task<CustomersListViewModel> GetCustomersForExport(CustomerPaginationViewModel model)
    {
        var query = _dbo.Customers.Include(c => c.Orders).AsQueryable();
        // var query = _dbo.Orders.Include(o => o.Customer).Include(o => o.Payments).AsQueryable();

        if (!string.IsNullOrEmpty(model.SearchTerm))
        {
            query = query.Where(customer => customer.Firstname.ToLower().Contains(model.SearchTerm.ToLower()));
        }

        // Apply Sorting using switch case
        query = model.SortBy switch
        {
            "Name" => model.SortOrder == "asc"
                ? query.OrderBy(c => c.Customerid)
                : query.OrderByDescending(u => u.Firstname),

            "Date" => model.SortOrder == "asc"
                ? query.OrderBy(u => u.Createdat)
                : query.OrderByDescending(u => u.Createdat),

            "Total Order" => model.SortOrder == "asc"
                ? query.OrderBy(u => u.Orders.Sum(o => o.Totalamount))
                : query.OrderByDescending(u => u.Orders.Sum(o => o.Totalamount)),

            _ => query.OrderBy(u => u.Customerid)
        };

        if (model.TimeLog != null && model.TimeLog != "All Time")
        {
            DateTime now = DateTime.Now;
            DateTime startDate = now;
            switch (model.TimeLog)
            {

                case "Last 7 days":
                    startDate = now.AddDays(-7);
                    break;
                case "Last 30 days":
                    startDate = now.AddDays(-30);
                    break;
                case "Current Month":
                    startDate = new DateTime(now.Year, now.Month, 1);
                    break;
                    // case "Custom Date":
                    //     startDate = 
            }
            query = query.Where(o => o.Createdat >= startDate);
        }

        int totalItems = await query.CountAsync();
        // int totalPages = (int)Math.Ceiling(totalItems / (double)model.PageSize);

        var paginatedItems = await query
            // .Skip((model.Page - 1) * model.PageSize)
            // .Take(model.PageSize)
            .Select(customer => new CustomerViewModel
            {
                Customerid = customer.Customerid,
                Firstname = customer.Firstname,
                Lastname = customer.Lastname,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedAt = customer.Createdat.ToString("yyyy-MM-dd HH:mm:ss"),
                TotalOrders = _dbo.Orders.Count(o => o.Customerid == customer.Customerid)
            })
            .ToListAsync();

        return new CustomersListViewModel
        {
            Customers = paginatedItems
        };
    }

    public async Task<CustomerViewModel> GetCustomerHistoryByCustomerId(int customerId)
    {
        IQueryable<Entity.Models.Order>? orders = _dbo.Orders.Include(od => od.Orderdetails).Where(c => c.Customerid == customerId);
        List<OrdersViewModel> customerOrders = new();
        foreach (var order in orders)
        {
            OrdersViewModel customerOrder = new()
            {
                Orderid = order.Orderid,
                Createdat = order.Createdat,
                OrderType = "DineIn",
                Status = order.Status,
                ItemsCount = order.Orderdetails.Count,
                TotalAmount = order.Totalamount
            };
            customerOrders.Add(customerOrder);
        }
        
        return await _dbo.Customers
                        .Where(c => c.Customerid == customerId)
                        .Select(customer => new CustomerViewModel
                        {
                            Customerid = customer.Customerid,
                            Firstname = customer.Firstname,
                            Lastname = customer.Lastname,
                            Email = customer.Email,
                            Phone = customer.Phone,
                            CreatedAt = customer.Createdat.ToString("yyyy-MM-dd HH:mm:ss"),
                            TotalOrders = _dbo.Orders.Count(o => o.Customerid == customer.Customerid),
                            MaxOrder = _dbo.Orders.Where(o => o.Customerid == customer.Customerid).Max(o => o.Totalamount),
                            AverageBill = _dbo.Orders.Where(o => o.Customerid == customer.Customerid).Average(o => o.Totalamount),
                            ComingSince = _dbo.Orders.Where(o => o.Customerid == customerId).Min(o => o.Createdat).ToString("yyyy-MM-dd HH:mm:ss"),
                            Visits = _dbo.Orders.Count(o => o.Customerid == customer.Customerid),
                            CustomerOrders = customerOrders
                        }).FirstAsync();
    }
}
