using Microsoft.EntityFrameworkCore;
using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using System.Threading.Tasks;

namespace PizzaShop.Repository.Implementations;

public class OrderAppRepository : IOrderAppRepository
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly ApplicationDbContext _dbo;
    public OrderAppRepository(ApplicationDbContext dbo, IOrderRepository orderRepository, IMenuRepository menuRepository)
    {
        _dbo = dbo;
        _orderRepository = orderRepository;
        _menuRepository = menuRepository;
    }

    public async Task<List<Order>> GetOrderDetailsByMenuCategory(int menuCategoryId)
    {
        try
        {
            if (menuCategoryId == 0)
            {
                List<Order>? orders = _dbo.Orders
                .Include(o => o.Orderdetails)
                .ThenInclude(od => od.Item)
                .Include(o => o.Tablegroupings)
                .ThenInclude(t => t.Table)
                .ThenInclude(t => t.Section)
                .Include(o => o.Orderdetails)
                .ThenInclude(od => od.Ordermodifierdetails)
                .ThenInclude(od => od.Item)
                .Where(o => !o.Isdeleted)
                .ToList();

                return orders;
            }
            else
            {
                List<Order>? orders = _dbo.Orders
                .Include(o => o.Orderdetails)
                .ThenInclude(od => od.Item)
                .Include(o => o.Tablegroupings)
                .ThenInclude(t => t.Table)
                .ThenInclude(t => t.Section)
                .Include(o => o.Orderdetails)
                .ThenInclude(od => od.Ordermodifierdetails)
                .ThenInclude(od => od.Item)
                .Where(o => !o.Isdeleted &&
                    o.Orderdetails.Any(od => od.Item.Categoryid == menuCategoryId))
                .ToList();

                return orders;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }

    public async Task<bool> MarkOrderAsReady(int orderId, List<OrderDetailItem> orderDetailsIds)
    {
        try
        {
            List<Orderdetail> orderDetails = await _dbo.Orderdetails.Where(od => od.Orderid == orderId).ToListAsync();
            List<int?> Ids = orderDetailsIds.Select(od => od.Id).ToList();

            if (orderDetails != null)
            {
                foreach (Orderdetail? orderDetail in orderDetails)
                {
                    if (orderDetailsIds.Select(od => od.Id).ToList().Contains(orderDetail.Itemid))
                    {
                        orderDetail.Availablequantity += orderDetailsIds.FirstOrDefault(od => od.Id == orderDetail.Itemid)?.Quantity;
                        if (orderDetail.Availablequantity == orderDetail.Quantity)
                        {
                            orderDetail.Status = "Ready";
                        }
                    }
                    _dbo.Orderdetails.Update(orderDetail);
                    await _dbo.SaveChangesAsync();
                }
            }
            // Update the order status to "Served" if all items are ready
            bool isAllItemReady = false;
            Order? order = await _dbo.Orders.Where(o => o.Orderid == orderId).Include(o => o.Orderdetails).Select(o => o).FirstOrDefaultAsync();
            foreach (var orderDetail in order.Orderdetails)
            {
                if (orderDetail.Status == "Ready")
                {
                    isAllItemReady = true;
                }
                else
                {
                    isAllItemReady = false;
                }
            }
            if (isAllItemReady)
            {
                order.Status = "Served";
                _dbo.Orders.Update(order);
                await _dbo.SaveChangesAsync();
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> MarkOrderAsInProgress(int orderId, List<OrderDetailItem> orderDetailsIds)
    {
        try
        {
            List<Orderdetail> orderDetails = await _dbo.Orderdetails.Where(od => od.Orderid == orderId).ToListAsync();
            List<int?> Ids = orderDetailsIds.Select(od => od.Id).ToList();

            if (orderDetails != null)
            {
                foreach (Orderdetail? orderDetail in orderDetails)
                {
                    if (orderDetailsIds.Select(od => od.Id).ToList().Contains(orderDetail.Itemid))
                    {
                        orderDetail.Availablequantity -= orderDetailsIds.FirstOrDefault(od => od.Id == orderDetail.Itemid)?.Quantity;
                        if (orderDetail.Availablequantity < orderDetail.Quantity)
                        {
                            orderDetail.Status = "InProgress";
                        }
                    }
                    _dbo.Orderdetails.Update(orderDetail);
                    await _dbo.SaveChangesAsync();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> AddWaitingToken(AddWaitingTokenForTableViewModel model)
    {
        if (model == null) return false;

        string[]? names = model.Name.Trim().Split(' ');
        string? firstName = names.FirstOrDefault() ?? string.Empty;
        string? lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : string.Empty;

        Customer customer = _dbo.Customers.FirstOrDefault(c => c.Email == model.Email);
        if (customer == null)
        {
            customer = new Customer
            {
                Firstname = firstName,
                Lastname = lastName,
                Email = model.Email.Trim().ToLower(),
                Phone = model.Phone,
                // Totalpersons = model.TotalPersons,
                Createdat = DateTime.Now,
                Createdby = 1
            };

            await _dbo.Customers.AddAsync(customer);
            await _dbo.SaveChangesAsync();
            // _dbo.Customers.Add(customer);
            // _dbo.SaveChanges();
        }

        Waitingticket? waitingToken = new()
        {
            Customerid = customer.Customerid,
            Sectionid = model.SectionId123,
            Noofpersons = model.TotalPersons,
            Createdat = DateTime.Now,
            Createdby = 1,
            Isdeleted = false
        };

        await _dbo.Waitingtickets.AddAsync(waitingToken);
        _dbo.SaveChanges();
        return true;
    }

    public async Task<List<Waitingticket>> GetWaitingCustomers(int sectionId)
    {
        return await _dbo.Waitingtickets.Where(c => c.Sectionid == sectionId).Include(c => c.Customer).Include(s => s.Section).ToListAsync();
    }

    public async Task<List<Table>> GetTableListByTableIds(List<int> Ids)
    {
        return await _dbo.Tables
                        .Where(t => Ids.Contains(t.Tableid))
                        .ToListAsync();
    }

    public async Task<Waitingticket> GetWaitingCustomerByWaitingTokenId(int waitingTokenId)
    {
        return _dbo.Waitingtickets
                        .Include(w => w.Customer)
                        .FirstOrDefault(t => t.Waitingticketid == waitingTokenId);
    }

    public async Task<bool> UpdateWaitingToken(Waitingticket? waitingTicket)
    {
        _dbo.Waitingtickets.Update(waitingTicket);
        _dbo.Customers.Update(waitingTicket.Customer);
        _dbo.SaveChanges();
        return true;
    }

    public async Task<bool> AddCustomerForWaitingList(Customer customer)
    {
        await _dbo.Customers.AddAsync(customer);
        _dbo.SaveChanges();
        return true;
    }

    public async Task<Order> GetOrderByCustomerId(int customerId)
    {
        Order order = _dbo.Orders.FirstOrDefault(o => o.Customerid == customerId && !o.Isdeleted);
        if (order == null)
            return null;

        return order;
    }

    public async Task<bool> AddOrder(Order order)
    {
        try
        {
            await _dbo.Orders.AddAsync(order);
            _dbo.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }

    public async Task<bool> UpdateOrder(Order order)
    {
        _dbo.Orders.Update(order);
        _dbo.SaveChanges();
        return true;
    }

    public async Task<bool> AddTableOrderMapping(int orderId, int tableId)
    {
        Tablegrouping tableOrderMapping = new()
        {
            Orderid = orderId,
            Tableid = tableId
        };
        await _dbo.Tablegroupings.AddAsync(tableOrderMapping);
        _dbo.SaveChanges();
        return true;
    }

    #region Waiting List

    public async Task<List<Waitingticket>> GetWaitingTokens()
    {
        return await _dbo.Waitingtickets
                        .Include(w => w.Customer)
                        .Include(s => s.Section)
                        .Where(w => !w.Isdeleted)
                        .OrderBy(w => w.Waitingticketid)
                        .ToListAsync();
    }

    //Same as GetWaitingCustomers
    public async Task<List<Waitingticket>> GetWaitingTokensBySectionId(int sectionId)
    {
        return await _dbo.Waitingtickets
                        .Include(w => w.Customer)
                        .Include(s => s.Section)
                        .Where(w => w.Sectionid == sectionId && !w.Isdeleted)
                        .OrderBy(w => w.Waitingticketid)
                        .ToListAsync();
    }

    public async Task<AddWaitingTokenForTableViewModel> GetWaitingTokenForUpdate(int tokenId)
    {
        Waitingticket? waitingToken = await _dbo.Waitingtickets
            .Include(w => w.Customer)
            .Include(w => w.Section)
            .FirstOrDefaultAsync(w => w.Waitingticketid == tokenId && !w.Isdeleted);

        if (waitingToken == null) return null;

        return new AddWaitingTokenForTableViewModel
        {
            CustomerId = waitingToken.Customerid,
            WaitingTokenId = waitingToken.Waitingticketid,
            SectionName = waitingToken.Section?.Sectionname,
            SectionId123 = waitingToken.Sectionid,
            Name = $"{waitingToken.Customer.Firstname} {waitingToken.Customer.Lastname}",
            Email = waitingToken.Customer.Email,
            Phone = waitingToken.Customer.Phone,
            TotalPersons = waitingToken.Noofpersons ?? 0,
        };
    }

    public async Task<List<Section>> GetSections()
    {
        return _dbo.Sections
        .Where(s => s.Isdeleted == false)
        .ToList();
    }

    public List<Table> GetTables(int sectionId)
    {
        var tables = _dbo.Tables
        .Where(t => t.Sectionid == sectionId && t.Isdeleted == false && t.Newstatus == 1)
        .ToList();

        return tables;
    }

    public async Task<Customer?> GetCustomerByEmail(string email)
    {
        return await _dbo.Customers.FirstOrDefaultAsync(c => c.Email == email);
    }

    #endregion

    #region Menu

    public List<Item> GetFilteredItems(int categoryId, string? searchText)
    {
        IOrderedQueryable<Item>? itemsQuery = _dbo.Items.Where(i => !i.Isdeleted && i.Ismodifiable == false).OrderBy(i => i.Itemid);

        if (categoryId == -1) // -1 = Favorite
            itemsQuery = itemsQuery.Where(i => i.Isfavorite == true).OrderBy(i => i.Itemid);

        if (categoryId > 0)
            itemsQuery = itemsQuery.Where(i => i.Categoryid == categoryId).OrderBy(i => i.Itemid);

        if (!string.IsNullOrWhiteSpace(searchText))
            itemsQuery = itemsQuery.Where(i => i.Itemname.ToLower().Contains(searchText.ToLower())).OrderBy(i => i.Itemid);

        return itemsQuery.ToList();
    }

    public string GetModifierNameById(int? modifierId)
    {
        return _dbo.Items
            .Where(i => i.Itemid == modifierId)
            .Select(i => i.Itemname)
            .FirstOrDefault();
    }

    public List<Menucategory> GetAllCategories()
    {
        return _dbo.Menucategories.Where(c => !c.Isdeleted).OrderBy(c => c.Menucategoryid).ToList();
    }

    public void ToggleFavorite(int itemId)
    {
        Item? item = _dbo.Items.Find(itemId);
        if (item != null)
        {
            item.Isfavorite = !item.Isfavorite;
            _dbo.SaveChanges();
        }
    }

    public async Task<ModifierSelectionModalViewModel> GetModifiersGroupedByItemAsync(int itemId)
    {
        List<ModifiersGroupForMenuViewModel>? result = await _dbo.Itemmodifiergroupmappings
            .Where(img => img.Itemid == itemId)
            .Include(img => img.Item)
            .Include(img => img.Modifiergroup)
            .Select(img => new ModifiersGroupForMenuViewModel
            {
                GroupName = img.Modifiergroup.Modifiername,
                Min = img.Minquantity ?? 0,
                Max = img.Maxquantity ?? 0,
                // Min = 3,
                // Max = 4,
                Modifiers = img.Modifiergroup.Itemmodifiergroupmappings.Where(img => img.Isitemmodifiable == true).Select(mgm => new ModifiersItemForMenuViewModel
                {
                    Id = mgm.Item.Itemid,
                    Name = mgm.Item.Itemname,
                    Price = (short)mgm.Item.Rate,
                    // ItemType = mgm.Item.Itemtype,
                    IsSelected = false
                }).ToList()
            }).ToListAsync();

        Item? item = await _dbo.Items.FindAsync(itemId);

        if (item != null)
        {
            ModifierSelectionModalViewModel? itemDetails = new ModifierSelectionModalViewModel
            {
                ItemId = item.Itemid,
                ItemName = item.Itemname,
                Quantity = item.Quantity,
                Rate = item.Rate ?? 0,
                TaxPercentage = item.Tax ?? 0,
                ModifierGroups = result
            };
            return itemDetails;
        }
        return new ModifierSelectionModalViewModel();
    }

    // / <summary>
    // / Returns basic customer details (Name, Phone, Email, NoOfPersons)
    // / for the active order.  Null if order or customer not found.
    // / </summary>
    public async Task<OrderCustomerViewModel?> GetOrderCustomerAsync(int orderId)
    {
        // Pull the order with its customer in a single round‑trip
        var order = await _dbo.Orders
                                  .Where(o => o.Orderid == orderId)
                                  .Select(o => new
                                  {
                                      o.Orderid,
                                      Customer = new
                                      {
                                          o.Customer.Customerid,
                                          Name = o.Customer.Firstname + " " + o.Customer.Lastname,
                                          Phone = o.Customer.Phone,
                                          Email = o.Customer.Email,
                                          Persons = _dbo.Orders
                                              .Where(o => o.Customerid == o.Customer.Customerid)
                                              .Select(o => o.Noofpersons)
                                              .FirstOrDefault()
                                      }
                                  })
                                  .FirstOrDefaultAsync();

        if (order == null || order.Customer == null)
            return null;

        return new OrderCustomerViewModel
        {
            OrderId = order.Orderid,
            CustomerId = order.Customer.Customerid,
            Name = order.Customer.Name,
            Phone = order.Customer.Phone,
            Email = order.Customer.Email,
            NoOfPersons = order.Customer.Persons
        };
    }


    public async Task<bool> UpdateCustomerAsync(UpdateCustomerViewModel vm)
    {
        // 1. load customer & ensure it belongs to that order
        Order? order = await _dbo.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Orderid == vm.OrderId &&
                                      o.Customerid == vm.CustomerId);

        if (order?.Customer == null) return false;

        // 2. apply changes
        string[]? names = (vm.Name ?? string.Empty)
            .Trim()
            .Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        Customer? c = order.Customer;
        c.Firstname = names.Length > 0 ? names[0] : string.Empty;
        c.Lastname = names.Length > 1 ? names[1] : string.Empty;
        c.Phone = vm.Phone;
        c.Email = vm.Email;
        // c.Totalpersons = vm.NoOfPersons; get totralpersons from order table

        _dbo.Customers.Update(c);
        await _dbo.SaveChangesAsync();
        return true;
    }

    public async Task<string?> GetAdminCommentAsync(int orderDetailId)
    {
        try
        {
            string? adminComment = await _dbo.Orders
                .Where(o => o.Orderid == orderDetailId)
                .Select(o => o.Admincomment)
                .FirstOrDefaultAsync();

            if (adminComment == null)
                return "";

            return adminComment;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching AdminComment: {ex.Message}");
            return "";
        }
    }

    public async Task<bool> SaveAdminCommentAsync(int orderDetailId, string adminComment)
    {
        try
        {
            Order? orderDetail = await _dbo.Orders.FirstOrDefaultAsync(o => o.Orderid == orderDetailId);
            if (orderDetail == null)
                return false;

            orderDetail.Admincomment = adminComment;
            await _dbo.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving AdminComment: {ex.Message}");
            return false;
        }
    }

    public async Task<string?> GetItemInstructionAsync(int orderId, int itemId)
    {
        try
        {
            string? instruction = _dbo.Orderdetails.FirstOrDefault(o => o.Orderid == orderId && o.Itemid == itemId).Iteminstruction;
            return instruction;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Instruction: {ex.Message}");
            return "";
        }
    }

    public async Task<bool> SaveItemInstructionAsync(int orderId, int itemId, string instruction)
    {
        try
        {
            Orderdetail? orderDetail = await _dbo.Orderdetails.FirstOrDefaultAsync(o => o.Orderid == orderId && o.Itemid == itemId);
            if (orderDetail == null)
                return false;

            orderDetail.Iteminstruction = instruction;
            await _dbo.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving Instruction: {ex.Message}");
            return false;
        }
    }

    // Get section name by orderId
    public string? GetSectionName(int? orderId)
    {
        if (orderId == null)
            return null;

        string? sectionName = (from tom in _dbo.Tablegroupings
                               join t in _dbo.Tables on tom.Tableid equals t.Tableid
                               join s in _dbo.Sections on t.Sectionid equals s.Sectionid
                               where tom.Orderid == orderId && !tom.Isdeleted
                               select s.Sectionname)
                          .FirstOrDefault();

        return sectionName;
    }

    // Get list of table names by orderId
    public List<string> GetTableNames(int? orderId)
    {
        if (orderId == null)
            return new List<string>();

        List<string>? tableNames = (from tom in _dbo.Tablegroupings
                                    join t in _dbo.Tables on tom.Tableid equals t.Tableid
                                    orderby t.Tablename
                                    where tom.Orderid == orderId && !tom.Isdeleted
                                    select t.Tablename)
                         .ToList();

        return tableNames;
    }

    public List<Taxesandfee> GetEnabledTaxes()
    {
        return _dbo.Taxesandfees
                       .Where(t => t.Isenabled == true && t.Isdeleted == false)
                       .ToList();
    }

    public async Task<List<Orderdetail>> SaveOrderItemsAsync(SaveOrderRequestViewModel model)
    {
        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = await _dbo.Database.BeginTransactionAsync();

        try
        {
            // Fetch all existing order details
            List<Orderdetail>? existingOrderDetails = await _dbo.Orderdetails
                .Where(od => od.Orderid == model.OrderId)
                .Include(od => od.Ordermodifierdetails)
                .ToListAsync();

            List<int>? incomingItemIds = model.OrderItems.Select(i => i.ItemId ?? 0).ToList();

            // 1. Delete items removed in UI
            List<Orderdetail>? itemsToDelete = existingOrderDetails
                .Where(od => !incomingItemIds.Contains(od.Itemid ?? 0))
                .ToList();

            foreach (Orderdetail? item in itemsToDelete)
            {
                _dbo.Ordermodifierdetails.RemoveRange(item.Ordermodifierdetails);
                _dbo.Orderdetails.Remove(item);
            }

            // 2. Add new items
            foreach (OrderItemForRowViewModel? item in model.OrderItems)
            {
                Orderdetail? existingItem = existingOrderDetails.FirstOrDefault(e => e.Itemid == item.ItemId);

                if (existingItem == null)
                {
                    // Add new item
                    Orderdetail? orderDetail = new()
                    {
                        Orderid = model.OrderId,
                        Itemid = item.ItemId,
                        Availablequantity = 0,
                        // Item.ItemName = item.ItemName,
                        // Inprogressquantity = item.Quantity,
                        // Readyquantity = 0,
                        Quantity = item.Quantity,
                        Price = item.Rate,
                        // Instructions = item.Instruction
                    };

                    await _dbo.Orderdetails.AddAsync(orderDetail);
                    _dbo.SaveChanges(); // To get ID

                    foreach (ModifierForMenuOrderViewModel? modifier in item.SelectedModifiers)
                    {
                        //Get orderDetailId 
                        // int orderDetailId = orderDetail.Orderdetailid;

                        // Orderdetail? orderDetail = await _dbo.Orderdetails.FirstOrDefaultAsync(od => od.Itemid == item.ItemId);
                        Ordermodifierdetail? orderDetailModifier = new Ordermodifierdetail
                        {
                            Orderdetailid = orderDetail.Orderdetailid,
                            Itemid = modifier.ModifierId,
                            // Modifieritemname = modifier.ModifierName,
                            Price = modifier.Rate,
                            Quantity = 1
                        };
                        await _dbo.Ordermodifierdetails.AddAsync(orderDetailModifier);
                    }
                }
                else
                {
                    // Update existing item
                    // if (existingItem.Availablequantity > item.Quantity)
                    // {
                    //     throw new InvalidOperationException($"Cannot reduce quantity below already prepared quantity ({existingItem.Availablequantity}).");
                    // }

                    existingItem.Quantity = item.Quantity;
                    // existingItem.Inprogressquantity = item.Quantity - existingItem.Availablequantity;
                    // // existingItem.Inprogressquantity = item.Quantity;
                    // existingItem.Availablequantity = item.Quantity - existingItem.Inprogressquantity;
                    existingItem.Price = item.Rate;
                    // existingItem.Instructions = item.Instruction;

                    // Update modifiers: remove old, add new
                    if (existingItem.Ordermodifierdetails.Count > 0)
                    {
                        // Remove old modifiers    
                        _dbo.Ordermodifierdetails.RemoveRange(existingItem.Ordermodifierdetails);
                        _dbo.SaveChanges();

                        foreach (ModifierForMenuOrderViewModel? modifier in item.SelectedModifiers)
                        {
                            Ordermodifierdetail? newModifier = new()
                            {
                                Orderdetailid = existingItem.Orderdetailid,
                                Itemid = modifier.ModifierId,
                                // Modifieritemname = modifier.ModifierName,
                                Price = modifier.Rate,
                                Quantity = 1
                            };
                            _dbo.Ordermodifierdetails.Add(newModifier);
                        }
                    }
                }
            }

            // Save item additions and deletions
            await _dbo.SaveChangesAsync();

            // 3. Update related dining tables
            List<Tablegrouping>? tablesToUpdate = await _dbo.Tablegroupings
                .Where(t => t.Orderid == model.OrderId && !t.Isdeleted)
                .ToListAsync();

            foreach (Tablegrouping? table in tablesToUpdate)
            {
                Table? diningTable = await _dbo.Tables.FirstOrDefaultAsync(x => x.Tableid == table.Tableid);
                if (diningTable != null)
                {
                    diningTable.Newstatus = 3; // 3 = "Running"
                }
            }

            // 4. Insert selected taxes
            foreach (int taxId in model.SelectedTaxIds)
            {
                Taxesandfee? tax = await _dbo.Taxesandfees.FindAsync(taxId);
                if (tax != null)
                {
                    Ordertaxmapping? taxmaapping = new()
                    {
                        Orderid = model.OrderId,
                        Taxid = tax.Taxid,
                        Taxname = tax.Taxname,
                        Taxtype = tax.Taxtype,
                        Taxamount = tax.Taxvalue //Need to be add SubTotal
                    };
                    await _dbo.Ordertaxmappings.AddAsync(taxmaapping);
                    await _dbo.SaveChangesAsync();

                    // _dbo.Ordertaxmappings.Add(new Ordertaxmapping
                    // {
                    //     Orderid = model.OrderId,
                    //     Taxid = tax.Taxid,
                    //     Taxname = tax.Taxname,
                    //     Taxtype = tax.Taxtype,
                    //     Taxamount = tax.Taxvalue //Need to be add SubTotal
                    // });
                }
            }

            await _dbo.SaveChangesAsync();
            await transaction.CommitAsync();

            // Return updated order details
            List<Orderdetail>? updatedOrderDetails = await _dbo.Orderdetails
                .Where(od => od.Orderid == model.OrderId)
                .Include(od => od.Item) // Include Item details like name, price
                .Include(od => od.Ordermodifierdetails)
                .ThenInclude(od => od.Item) // Include applied modifiers
                .ToListAsync();

            return updatedOrderDetails;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SaveOrderItemsAsync: {ex.Message}");
            await transaction.RollbackAsync();
            throw new Exception("Error saving order items");
        }
    }

    public async Task<List<SaveOrderDetailViewModel>?> CompleteOrderItemsAsync(int orderId)
    {
        List<Orderdetail>? updatedOrderDetails = await _dbo.Orderdetails
                .Where(od => od.Orderid == orderId)
                .Include(od => od.Item) // Include Item details like name, price
                .Include(od => od.Ordermodifierdetails)
                .ThenInclude(od => od.Item) // Include applied modifiers
                .ToListAsync();

        List<SaveOrderDetailViewModel>? orderDetails = updatedOrderDetails
            .Select(od => new SaveOrderDetailViewModel
            {
                Id = od.Orderid ?? 0,
                ItemId = od.Itemid ?? 0,
                ItemName = od.Item.Itemname,
                Quantity = od.Quantity,
                AvailableQuantity = od.Availablequantity,
                Rate = od.Price,
                Modifiers = od.Ordermodifierdetails.Select(mod => new SaveModifierViewModel
                {
                    ModifierId = mod.Itemid ?? 0,
                    ModifierName = mod.Item.Itemname,
                    Rate = mod.Price
                }).ToList()
            }).ToList();

        //Check if all Items are ready
        bool allItemsReady = orderDetails.All(od => od.Quantity == od.AvailableQuantity);
        if (allItemsReady)
        {
            // Update order status to "Ready"
            Order? order = await _dbo.Orders.FindAsync(orderId);
            if (order != null && order.Status == "Served")
            {
                order.Status = "Completed";
                _dbo.Orders.Update(order);
                await _dbo.SaveChangesAsync();


            }

            if (order.Status == "Completed")
            {
                // Update table status to "Available" 
                List<Tablegrouping>? tablesToUpdate = await _dbo.Tablegroupings
                    .Where(t => t.Orderid == orderId && !t.Isdeleted)
                    .Include(t => t.Table)
                    .ThenInclude(t => t.Section)
                    .ToListAsync();
                foreach (Tablegrouping? table in tablesToUpdate)
                {
                    Table? diningTable = await _dbo.Tables.FirstOrDefaultAsync(x => x.Tableid == table.Tableid);
                    if (diningTable != null)
                    {
                        diningTable.Newstatus = 1; // 1 = "Available"
                    }
                }
                await _dbo.SaveChangesAsync();
            }
        }
        else
        {
            return null;
        }

        return orderDetails;
    }

    public async Task<List<SaveOrderDetailViewModel>?> CancelOrderItemsAsync(int orderId)
    {
        List<Orderdetail>? updatedOrderDetails = await _dbo.Orderdetails
                .Where(od => od.Orderid == orderId)
                .Include(od => od.Item) // Include Item details like name, price
                .Include(od => od.Ordermodifierdetails)
                .ThenInclude(od => od.Item) // Include applied modifiers
                .ToListAsync();

        List<SaveOrderDetailViewModel>? orderDetails = updatedOrderDetails
            .Select(od => new SaveOrderDetailViewModel
            {
                Id = od.Orderid ?? 0,
                ItemId = od.Itemid ?? 0,
                ItemName = od.Item.Itemname,
                Quantity = od.Quantity,
                AvailableQuantity = od.Availablequantity,
                Rate = od.Price,
                Modifiers = od.Ordermodifierdetails.Select(mod => new SaveModifierViewModel
                {
                    ModifierId = mod.Itemid ?? 0,
                    ModifierName = mod.Item.Itemname,
                    Rate = mod.Price
                }).ToList()
            }).ToList();

        //Check if all Items are ready
        bool allItemsNotReady = orderDetails.All(od => od.AvailableQuantity == 0);
        if (allItemsNotReady)
        {
            // Update order status to "Cancel"
            Order? order = await _dbo.Orders.FindAsync(orderId);
            if (order != null && order.Status != "Served")
            {
                order.Status = "Canceled";
                _dbo.Orders.Update(order);
                await _dbo.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Order is already served, cannot cancel.");
            }
        }
        else
        {
            return null;
        }

        return orderDetails;
    }

    public async Task<Order> GetOrderByIdAsync(int orderId)
    {
        return await _dbo.Orders
            .Include(o => o.Orderdetails)
                .ThenInclude(od => od.Item) // Include Item details like name, price
            .Include(o => o.Orderdetails)
                .ThenInclude(od => od.Ordermodifierdetails) // Include applied modifiers
            .FirstAsync(o => o.Orderid == orderId);
    }

    public async Task<bool> DeleteOrderDetailByCompositeMatchAsync(DeleteOrderDetailRequest request)
    {
        List<Orderdetail>? possibleMatches = await _dbo.Orderdetails
            .Include(od => od.Ordermodifierdetails)
            .Where(od => od.Orderid == request.OrderId &&
                         od.Itemid == request.ItemId &&
                         od.Quantity == request.Quantity
                        //  && od.Instructions == request.Instruction
                        )
            .ToListAsync();

        foreach (Orderdetail? detail in possibleMatches)
        {
            List<Ordermodifierdetail>? dbMods = detail.Ordermodifierdetails.OrderBy(m => m.Itemid).ToList();
            List<SaveModifierViewModel>? reqMods = request.SelectedModifiers.OrderBy(m => m.ModifierId).ToList();

            if (dbMods.Count != reqMods.Count)
                continue;

            bool allMatch = dbMods.Zip(reqMods, (db, req) =>
                db.Itemid == req.ModifierId
                //  && db.Modifieritemname == req.ModifierName
                && db.Price == req.Rate
            ).All(result => result);

            if (allMatch)
            {
                _dbo.Ordermodifierdetails.RemoveRange(detail.Ordermodifierdetails);
                _dbo.Orderdetails.Remove(detail);
                await _dbo.SaveChangesAsync();
                return true;
            }
        }

        return false;
    }

    public async Task<int?> GetReadyQuantity(int itemId, int orderId)
    {
        try
        {
            int? readyQuantity = await _dbo.Orderdetails
                .Where(od => od.Itemid == itemId && od.Orderid == orderId)
                .Select(od => od.Quantity)
                .FirstAsync();

            if (readyQuantity == null)
            {
                Console.WriteLine($"success fetching ReadyQuantity: ");
                    return 0;
            }

            return readyQuantity;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching ReadyQuantity: {ex.Message}");
            return 0;
        }
    }

    #endregion

}