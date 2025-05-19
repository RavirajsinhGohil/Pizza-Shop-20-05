using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;

namespace PizzaShop.Repository.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbo;
    public OrderRepository(ApplicationDbContext dbo)
    {
        _dbo = dbo;
    }

    public async Task<OrdersViewModel> GetOrderById(int orderId)
    {
        return await _dbo.Orders
                .Where(o => o.Orderid == orderId)
                .Select(o => new OrdersViewModel
                {
                    Orderid = o.Orderid,
                    Customerid = o.Customerid,
                    tableId = o.Tableid,
                    CustomerName = o.Customer.Firstname,
                    Status = o.Status,
                    PaymentMode = o.Paymentmode,
                    InvoiceId = _dbo.Invoices.FirstOrDefault(i => i.Orderid == o.Customer.Customerid).Invoiceid,
                    TotalAmount = o.Totalamount,
                    Createdat = o.Createdat,
                    CreatedBy = o.Createdby.ToString(),
                    NoOfPersons = o.Noofpersons
                    // UpdatedAt = o.Updatedat,
                    // UpdatedBy = o.Updatedby.ToString()
                }).FirstAsync();
    }

    public async Task<CustomerViewModel> GetCustomerById(int customerId)
    {
        return await _dbo.Customers
                        .Where(c => c.Customerid == customerId)
                        .Select(c => new CustomerViewModel
                        {
                            Customerid = c.Customerid,
                            Firstname = c.Firstname,
                            Lastname = c.Lastname,
                            Email = c.Email,
                            Phone = c.Phone
                        }).FirstAsync();
    }

    public async Task<List<OrdersViewModel>> GetOrdersListModel()
    {
        return await _dbo.Orders
            .Where(o => !o.Isdeleted)
            .OrderBy(o => o.Orderid)
            .Select(o => new OrdersViewModel
            {
                Orderid = o.Orderid,
                Customerid = o.Customerid,
                CustomerName = _dbo.Customers.FirstOrDefault(c => c.Customerid == o.Customerid).Firstname,
                Status = o.Status,
                PaymentMode = o.Paymentmode,
                Rating = o.Avgrating,
                TotalAmount = o.Orderid,
                Createdat = o.Createdat
            })
            .ToListAsync();
    }

    public async Task<OrdersListViewModel> GetOrderByPaginationAsync(string searchTerm, int page, int pageSize, string SortBy, string SortOrder, string statusLog, string timeLog, string fromDate, string toDate)
    {
        var query = _dbo.Orders.Include(o => o.Customer).AsQueryable();
        // var query = _dbo.Orders.Include(o => o.Customer).Include(o => o.Payments).AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(order => order.Orderid.ToString().ToLower().Contains(searchTerm.ToLower()) || order.Customer.Firstname.ToLower().Contains(searchTerm.ToLower()));
        }

        if (statusLog != null && statusLog != "All Status")
        {
            query = query.Where(o => o.Status == statusLog);
        }

        // Apply Sorting using switch case
        query = SortBy switch
        {
            "OrderId" => SortOrder == "asc"
                ? query.OrderBy(u => u.Orderid)
                : query.OrderByDescending(u => u.Orderid),

            "CreatedAt" => SortOrder == "asc"
                ? query.OrderBy(u => u.Createdat)
                : query.OrderByDescending(u => u.Createdat),

            "CustomerName" => SortOrder == "asc"
                ? query.OrderBy(u => u.Customer.Firstname)
                : query.OrderByDescending(u => u.Customer.Firstname),

            "TotalAmount" => SortOrder == "asc"
                ? query.OrderBy(u => u.Totalamount)
                : query.OrderByDescending(u => u.Totalamount),

            _ => query.OrderBy(u => u.Orderid)
        };

        if (timeLog != null && timeLog != "All Time")
        {
            DateTime now = DateTime.Now;
            DateTime startDate = now;
            switch (timeLog)
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
            }

            query = query.Where(o => o.Createdat >= startDate);
        }

        // Apply custom date filter
        if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime from))
        {
            query = query.Where(o => o.Createdat >= from);
        }

        if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime to))
        {
            query = query.Where(o => o.Createdat <= to);
        }

        int totalItems = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var paginatedItems = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(order => new OrdersViewModel
            {
                Orderid = order.Orderid,
                Customerid = order.Customerid,
                CustomerName = _dbo.Customers.FirstOrDefault(c => c.Customerid == order.Customerid).Firstname,
                Status = order.Status,
                PaymentMode = order.Paymentmode,
                Rating = order.Avgrating,
                TotalAmount = order.Totalamount,
                Createdat = order.Createdat
            })
            .ToListAsync();

        return new OrdersListViewModel
        {
            orders = paginatedItems,
            CurrentPage = page,
            totalItems = totalItems,
            TotalPages = totalPages,
            PageSize = pageSize,
            FromRec = (page - 1) * pageSize + 1,
            ToRec = Math.Min(page * pageSize, totalItems)
        };
    }

    public async Task<OrdersListViewModel> GetOrdersForExport(string searchTerm, string statusLog, string timeLog)
    {
        var query = _dbo.Orders.Include(o => o.Customer).AsQueryable();
        // var query = _dbo.Orders.Include(o => o.Customer).Include(o => o.Payments).AsQueryable();
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(order => order.Orderid.ToString().ToLower().Contains(searchTerm.ToLower()));
            //On customer Name search is remaining
        }

        if (statusLog != null && statusLog != "All Status")
        {
            query = query.Where(o => o.Status == statusLog);
        }

        if (timeLog != null && timeLog != "All Time")
        {
            DateTime now = DateTime.Now;
            DateTime startDate = now;
            switch (timeLog)
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
            }

            query = query.Where(o => o.Createdat >= startDate);
        }

        int totalItems = await query.CountAsync();

        var paginatedItems = await query
            .Take(totalItems)
            .Select(order => new OrdersViewModel
            {
                Orderid = order.Orderid,
                Customerid = order.Customerid,
                CustomerName = _dbo.Customers.FirstOrDefault(c => c.Customerid == order.Customerid).Firstname,
                Status = order.Status,
                PaymentMode = order.Paymentmode,
                Rating = order.Avgrating,
                TotalAmount = order.Totalamount,
                Createdat = order.Createdat
            })
            .OrderBy(o => o.Orderid)
            .ToListAsync();

        return new OrdersListViewModel
        {
            orders = paginatedItems
        };
    }

    public async Task<List<TableViewModel>> GetTablesByOrderId(int orderId)
    {
        return await _dbo.Tablegroupings
                        .Where(t => t.Orderid == orderId && !t.Isdeleted)
                        .OrderBy(t => t.Tablegroupingid)
                        .Join(_dbo.Tables,
                        mapping => mapping.Tableid,
                        table => table.Tableid,
                        (mapping, table) => new TableViewModel
                        {
                            TableId = table.Tableid,
                            TableName = table.Tablename,
                            SectionId = table.Sectionid,
                            SectionName = _dbo.Sections.FirstOrDefault(s => s.Sectionid == table.Sectionid).Sectionname,
                            Capacity = table.Capacity,
                            Status = table.Status
                        }
                        ).ToListAsync();
    }

    public async Task<List<ItemViewModel>> GetItemsByOrderId(int orderId)
    {
        return await _dbo.Orderdetails
                        .Where(o => o.Orderid == orderId)
                        .OrderBy(o => o.Orderdetailid)
                        .Join(_dbo.Items,
                        mapping => mapping.Itemid,
                        item => item.Itemid,
                        (mapping, item) => new ItemViewModel
                        {
                            Itemid = item.Itemid,
                            Name = item.Itemname,
                            Rate = item.Rate,
                            CategoryId = item.Categoryid,
                            CategoryName = _dbo.Menucategories.FirstOrDefault(c => c.Menucategoryid == item.Categoryid).Categoryname,
                            OrderDetailId = mapping.Orderdetailid,
                            OrderQuantity = mapping.Quantity,
                            OrderPrice = mapping.Price,
                            Modifiers = _dbo.Ordermodifierdetails
                                        .Where(c => c.Orderdetailid == mapping.Orderdetailid)
                                        .Join(_dbo.Items,
                                        mapping => mapping.Itemid,
                                        item => item.Itemid,
                                        (mapping, item) => new ModifiersViewModel
                                        {
                                            ModifierId = item.Itemid,
                                            ModifierGroupId = item.Categoryid,
                                            Name = item.Itemname,
                                            Rate = item.Rate ?? 0,
                                            Quantity = item.Quantity ?? 0,
                                            Isdeleted = item.Isdeleted,
                                            OrderQuantity = mapping.Quantity,
                                            OrderPrice = mapping.Price
                                        }).ToList()
                        }
                        ).ToListAsync();
    }

    public async Task<List<ModifiersViewModel>> GetModifiersByOrderId(int orderId)
    {
        return await _dbo.Ordermodifierdetails
                                    .Join(_dbo.Orderdetails,
                                        omd => omd.Orderdetailid,
                                        od => od.Orderdetailid,
                                        (omd, od) => new { omd, od })
                                    .Where(x => x.od.Orderid == orderId)
                                    .OrderBy(o => o.omd.Orderdetailid)
                                    .Join(_dbo.Items,
                                        x => x.omd.Itemid,
                                        item => item.Itemid,
                                        (x, item) => new ModifiersViewModel
                                        {
                                            ModifierId = item.Itemid,
                                            ModifierGroupId = item.Categoryid,
                                            Name = item.Itemname,
                                            Rate = item.Rate,
                                            OrderDetailId = x.omd.Orderdetailid,
                                            OrderQuantity = x.omd.Quantity,
                                            OrderPrice = x.omd.Price,
                                        })
                                    .ToListAsync();
    }
}



// _dbo.Itemmodifiergroupmappings
//                 .Where(c => c.Modifiergroupid == id)
//                 .Join(_dbo.Items,
//                 mapping => mapping.Itemid,
//                 item => item.Itemid,
//                 (mapping, item) => new ModifiersViewModel
//                 {
//                     ModifierId = item.Itemid,
//                     ModifierGroupId = item.Categoryid,
//                     Name = item.Itemname,
//                     Unit = item.Unit,
//                     Rate = item.Rate ?? 0,
//                     Quantity = item.Quantity,
//                     Isdeleted = item.Isdeleted
//                 }).ToListAsync();



