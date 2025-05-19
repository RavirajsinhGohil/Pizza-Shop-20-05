using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;
using System.Threading.Tasks;

namespace PizzaShop.Service.Implementations;

public class OrderAppService : IOrderAppService
{
    private readonly IMenuService _menuService;
    private readonly ISectionService _sectionService;
    private readonly IOrderService _orderService;
    private readonly IOrderAppRepository _orderAppRepository;

    public OrderAppService(IMenuService menuService, ISectionService sectionService, IOrderService orderService, IOrderAppRepository orderAppRepository)
    {
        _menuService = menuService;
        _orderService = orderService;
        _orderAppRepository = orderAppRepository;
        _sectionService = sectionService;
    }

    #region KOT Module

    public async Task<List<KOTOrderCardViewModel>> GetOrderDetailsForAllCategories(int categoryId, bool inProgress)
    {
        List<Order> orders = await _orderAppRepository.GetOrderDetailsByMenuCategory(categoryId); // no categoryId here

        List<KOTOrderCardViewModel> orderCards = new List<KOTOrderCardViewModel>();

        foreach (Order? order in orders)
        {
            List<Table?>? tables = order.Tablegroupings.Select(tg => tg.Table).ToList();
            string? tableName = string.Join(" ", tables.Select(t => t.Tablename).ToList());
            string? sectionName = tables.Select(t => t.Section).FirstOrDefault()?.Sectionname;
            List<KOTOrderItemViewModel>? items = order.Orderdetails
                .Where(od => od.Item != null)
                .Select(od => new KOTOrderItemViewModel
                {
                    ItemId = od.Itemid,
                    ItemName = od.Item?.Itemname,
                    Quantity = od.Quantity,
                    ItemInstruction = od.Iteminstruction,
                    // Category = ,
                    // Category.Id = od.Item.Categoryid,
                    // CategoryName = od.Item.Category?.Name,
                    Modifiers = od.Ordermodifierdetails.Select(mod => new KOTOrderModifierViewModel
                    {
                        ModifierId = mod.Itemid,
                        ModifierName = mod.Item?.Itemname
                    }).ToList(),
                    InProgressQuantity = inProgress
                        ? od.Quantity - od.Availablequantity
                        : od.Availablequantity
                }).ToList();

            if (items.Count == 0)
                continue;

            KOTOrderCardViewModel? card = new KOTOrderCardViewModel
            {
                OrderId = order.Orderid,
                OrderInstruction = order.Admincomment,
                SectionTable = new List<KOTOrderSectionTableViewModel>
            {
                new() {
                    TableName = tableName,
                    // TableName = order.Table?.Tablename,
                    // TableName = order.Tablegroupings
                    //     .Select(tg => tg.Table?.Tablename)
                    //     .ToList().ToString() ?? "",
                        
                    SectionName = sectionName ?? ""
                }
            },
                CreatedAt = order.Createdat,
                Status = order.Status,
                Items = items
            };

            orderCards.Add(card);
        }

        return orderCards;
    }

    public async Task<List<KOTOrderCardViewModel>> GetOrderDetailByCategoryId(int categoryId, bool inProgress)
    {
        List<Order> orders = await _orderAppRepository.GetOrderDetailsByMenuCategory(categoryId);

        if (orders == null || orders.Count == 0)
        {
            return new List<KOTOrderCardViewModel>();
        }

        if (categoryId == 0)
        {
            List<KOTOrderCardViewModel> cards = await GetOrderDetailsForAllCategories(categoryId, inProgress);
            return cards;
        }

        CategoryViewModel category = await _menuService.GetCategorybyId(categoryId);

        List<KOTOrderCardViewModel>? OrderCards = new();
        if (orders != null && orders.Count > 0)
        {
            foreach (Order? order in orders)
            {
                List<Table?>? tables = order.Tablegroupings.Select(tg => tg.Table).ToList();
                string? tableName = string.Join(" ", tables.Select(t => t.Tablename).ToList());
                string? sectionName = tables.Select(t => t.Section).FirstOrDefault()?.Sectionname;
                KOTOrderCardViewModel card = new KOTOrderCardViewModel
                {
                    OrderId = order.Orderid,
                    SectionTable = new List<KOTOrderSectionTableViewModel>
                    {
                        new() {
                            TableName = tableName,
                            SectionName = sectionName ?? "",
                        }
                    },
                    CreatedAt = order.Createdat,
                    Categoryid = categoryId,
                    CategoryName = category.Name,
                    Status = order.Status,
                    OrderInstruction = order.Admincomment,
                    // OrderDetailsIds = order
                    // OrderInstruction = order.Orderinstruction,
                    // card.ItemInstruction = 
                    Items = order.Orderdetails
                        .Where(od => od.Item.Categoryid == categoryId) // Filter by category just in case
                        .Select(od => new KOTOrderItemViewModel
                        {
                            ItemId = od.Itemid,
                            ItemName = od.Item?.Itemname,
                            Quantity = od.Quantity,
                            Modifiers = od.Ordermodifierdetails
                                        .Select(mod => new KOTOrderModifierViewModel
                                        {
                                            ModifierId = mod.Itemid,
                                            ModifierName = mod.Item?.Itemname
                                        }).ToList(),
                            InProgressQuantity = inProgress ? od.Quantity - (od.Availablequantity ?? 0) : od.Availablequantity,
                            ItemInstruction = od.Iteminstruction
                            // Add more fields as needed from OrderDetail or Item
                        }).ToList(),
                };
                OrderCards.Add(card);
            }
        }
        return OrderCards;
    }

    public async Task<bool> MarkOrderAsReady(int orderId, List<OrderDetailItem> orderDetailsIds)
    {
        // Call the service to mark the order as ready
        return await _orderAppRepository.MarkOrderAsReady(orderId, orderDetailsIds);
    }

    public async Task<bool> MarkOrderAsInProgress(int orderId, List<OrderDetailItem> orderDetailsIds)
    {
        // Call the service to mark the order as in progress
        return await _orderAppRepository.MarkOrderAsInProgress(orderId, orderDetailsIds);
    }

    #endregion

    #region Table Module

    public async Task<OrderAppTableModuleViewModel> GetTabelModuleData()
    {
        OrderAppTableModuleViewModel model = new()
        {
            Sections = await _sectionService.GetAllSections(),
        };

        return model;
    }

    public async Task<bool> AddWaitingToken(AddWaitingTokenForTableViewModel model)
    {
        return await _orderAppRepository.AddWaitingToken(model);
    }

    public async Task<List<WaitingCustomerViewModel>> GetWaitingCustomers(int sectionId)
    {
        List<Waitingticket>? waitingTickets = await _orderAppRepository.GetWaitingCustomers(sectionId);

        List<WaitingCustomerViewModel>? waitingCustomers = new();
        foreach (Waitingticket? waitingTicket in waitingTickets)
        {
            WaitingCustomerViewModel waitingCustomer = new()
            {
                Id = waitingTicket.Customerid,
                WaitingTicketId = waitingTicket.Waitingticketid,
                Name = waitingTicket.Customer.Firstname,
                Email = waitingTicket.Customer.Email,
                Mobile = waitingTicket.Customer.Phone,
                NoOfPersons = waitingTicket.Noofpersons,
                SectionName = waitingTicket.Section.Sectionname,
                SectionId = sectionId
            };
            waitingCustomers.Add(waitingCustomer);
        }
        return waitingCustomers;
    }

    public async Task<int?> AssignCustomerToTableAsync(AssignTableViewModel model)
    {
        if (model == null) return null;

        /*  1. Parse selected table ids  */
        model.TableIds = model.SelectedTableId?
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(id => int.TryParse(id, out var n) ? n : (int?)null)
                            .OfType<int>()    // remove nulls
                            .ToList();

        if (model.TableIds == null || model.TableIds.Count == 0)
            return null;

        /*  2. Load tables  */
        List<Table>? tables = await _orderAppRepository.GetTableListByTableIds(model.TableIds);

        if (tables.Count == 0) return null;

        /*  3. Resolve / create customer  */
        Customer customer;

        if (model.Id is int waitingTokenId)
        {
            Waitingticket? waitingToken = await _orderAppRepository.GetWaitingCustomerByWaitingTokenId(waitingTokenId);

            if (waitingToken?.Customer == null) return null;

            customer = waitingToken.Customer;

            waitingToken.Tableassigntime = DateTime.Now;
            waitingToken.Isactive = false;
            await _orderAppRepository.UpdateWaitingToken(waitingToken);
        }
        else
        {
            customer = new Customer
            {
                Firstname = model.Name,
                Lastname = string.Empty,
                Email = model.Email,
                Phone = model.Mobile,
                // Totalpersons = model.NoOfPersons,
                Createdat = DateTime.Now,
                Createdby = 1
            };

            await _orderAppRepository.AddCustomerForWaitingList(customer);
        }

        /*  4. Open new order  */
        Order? order = await _orderAppRepository.GetOrderByCustomerId(customer.Customerid);

        // if (order == null)
        // {
        order = new Order
        {
            Customerid = customer.Customerid,
            Createdat = DateTime.Now,
            Createdby = 1,
            Status = "Pending",
            Paymentmode = "Pending",
            Noofpersons = model.NoOfPersons,
            // Tableid = tables.FirstOrDefault()?.Tableid,
        };

        await _orderAppRepository.AddOrder(order);
        // }
        // else
        // {
        //     order.Noofpersons = model.NoOfPersons;
        //     order.Status = "Pending";
        //     order.Paymentmode = "Pending";
        //     order.Updatedat = DateTime.Now;
        //     order.Updatedby = 1;

        //     await _orderAppRepository.UpdateOrder(order);
        // }

        // /*  5. Assign tables & mapping rows  */
        foreach (var table in tables)
        {
            table.Status = false;
            table.Newstatus = 2; // TableStatus.Assigned;
            table.Assignedtime = DateTime.Now;

            await _orderAppRepository.AddTableOrderMapping(order.Orderid, table.Tableid);
        }

        int orderId = order.Orderid;
        /*  6. Done  return new OrderId  */
        return 1;
    }

    #endregion

    #region Waiting List

    public async Task<WaitingListViewModel> GetWaitingListSections()
    {
        List<SectionNameViewModel>? sections = _sectionService.GetSectionList().ToList();
        List<SectionModuleViewModel>? sectionModules = new();

        foreach (var section in sections)
        {
            List<WaitingTokenViewModel> waitingTokens = new(); // Create a new list for each section
            List<Waitingticket>? waitingTickets = await _orderAppRepository.GetWaitingCustomers(section.SectionId);

            if (waitingTickets != null && waitingTickets.Count > 0)
            {
                foreach (var waitingTicket in waitingTickets)
                {
                    WaitingTokenViewModel waitingToken = new()
                    {
                        TokenId = waitingTicket.Waitingticketid,
                        CreatedTime = waitingTicket.Createdat ?? DateTime.Now,
                        // AssignedTime = waitingTicket.Tableassigntime ?? DateTime.Now,
                        CustomerName = waitingTicket.Customer?.Firstname,
                        CustomerPhone = waitingTicket.Customer?.Phone,
                        CustomerEmail = waitingTicket.Customer?.Email,
                        NoOfPeople = waitingTicket.Noofpersons
                    };
                    waitingTokens.Add(waitingToken);
                }
            }

            SectionModuleViewModel sectionModule = new()
            {
                SectionId = section.SectionId,
                SectionName = section.SectionName,
                WaitingTokenList = waitingTokens
            };

            sectionModules.Add(sectionModule);
        }

        return new WaitingListViewModel
        {
            Sections = sectionModules
        };
    }

    public async Task<List<WaitingTokenViewModel>> GetWaitingTokensBySectionId(int sectionId)
    {
        if (sectionId == 0)
        {
            List<Waitingticket>? waitingTokens = await _orderAppRepository.GetWaitingTokens();
            List<WaitingTokenViewModel>? waitingTokensViewModel = new();
            foreach (var waitingToken in waitingTokens)
            {
                WaitingTokenViewModel waitingTokenViewModel = new()
                {
                    TokenId = waitingToken.Waitingticketid,
                    CreatedTime = waitingToken.Createdat ?? DateTime.Now,
                    AssignedTime = waitingToken.Tableassigntime,
                    CustomerName = waitingToken.Customer?.Firstname,
                    CustomerPhone = waitingToken.Customer?.Phone,
                    CustomerEmail = waitingToken.Customer?.Email,
                    NoOfPeople = waitingToken.Noofpersons,
                    SectionId = waitingToken.Sectionid,
                    SectionName = waitingToken.Section?.Sectionname
                };
                waitingTokensViewModel.Add(waitingTokenViewModel);
            }
            return waitingTokensViewModel;
        }
        else
        {
            List<Waitingticket>? waitingTokens = await _orderAppRepository.GetWaitingTokensBySectionId(sectionId);
            List<WaitingTokenViewModel>? waitingTokensViewModel = new();
            foreach (var waitingToken in waitingTokens)
            {
                WaitingTokenViewModel waitingTokenViewModel = new()
                {
                    TokenId = waitingToken.Waitingticketid,
                    CreatedTime = waitingToken.Createdat ?? DateTime.Now,
                    AssignedTime = waitingToken.Tableassigntime,
                    CustomerName = waitingToken.Customer?.Firstname,
                    CustomerPhone = waitingToken.Customer?.Phone,
                    CustomerEmail = waitingToken.Customer?.Email,
                    NoOfPeople = waitingToken.Noofpersons,
                    SectionId = waitingToken.Sectionid,
                    SectionName = waitingToken.Section?.Sectionname
                };
                waitingTokensViewModel.Add(waitingTokenViewModel);
            }
            return waitingTokensViewModel;
        }
    }

    public async Task<AddWaitingTokenForTableViewModel> GetWaitingTokenById(int tokenId)
    {
        AddWaitingTokenForTableViewModel? waitingTicket = await _orderAppRepository.GetWaitingTokenForUpdate(tokenId);
        if (waitingTicket == null) return null;
        return waitingTicket;
    }

    public async Task<bool> UpdateWaitingToken(AddWaitingTokenForTableViewModel model)
    {
        Waitingticket? waitingTicket = await _orderAppRepository.GetWaitingCustomerByWaitingTokenId(model.WaitingTokenId);
        waitingTicket.Waitingticketid = model.WaitingTokenId;
        waitingTicket.Customerid = model.CustomerId;
        waitingTicket.Sectionid = model.SectionId123;
        waitingTicket.Noofpersons = model.TotalPersons;
        waitingTicket.Tableassigntime = DateTime.Now; //Table assigntime
        waitingTicket.Updatedat = DateTime.Now;
        waitingTicket.Updatedby = 1;
        waitingTicket.Customer.Firstname = model.Name.Split(' ')[0];
        waitingTicket.Customer.Lastname = model.Name.Split(' ').Length > 1 ? model.Name.Split(' ')[1] : string.Empty;
        waitingTicket.Customer.Email = model.Email;
        waitingTicket.Customer.Phone = model.Phone;
        waitingTicket.Customer.Updatedat = DateTime.Now;
        waitingTicket.Customer.Updatedby = 1;

        return await _orderAppRepository.UpdateWaitingToken(waitingTicket);
    }

    public async Task<bool> DeleteWaitingToken(int tokenId)
    {
        Waitingticket? waitingToken = await _orderAppRepository.GetWaitingCustomerByWaitingTokenId(tokenId);
        if (waitingToken == null) return false;

        waitingToken.Isdeleted = true;
        waitingToken.Updatedat = DateTime.Now;
        waitingToken.Updatedby = 1;

        return await _orderAppRepository.UpdateWaitingToken(waitingToken);
    }

    public async Task<List<Section>> GetSections()
    {
        var sections = await _orderAppRepository.GetSections();
        return sections;
    }

    public List<Table> GetTables(int sectionId)
    {
        return _orderAppRepository.GetTables(sectionId);
    }

    public async Task<CustomerViewModel?> GetCustomerByEmail(string email)
    {
        Customer? customer = await _orderAppRepository.GetCustomerByEmail(email);
        if (customer == null) return null;

        CustomerViewModel? viewModel = new()
        {
            Customerid = customer.Customerid,
            Email = customer.Email,
            Firstname = customer.Firstname,
            Lastname = customer.Lastname,
            Phone = customer.Phone
        };
        return viewModel;
    }

    // public List<Section> GetSections()
    // {
    //     return _context.Sections
    //     .Where(s => !s.Isdeleted)
    //     .ToList();
    // }

    // public List<Diningtable> GetTables(int sectionId)
    // {
    //     var tables = _context.Diningtables
    //     .Where(t => t.SectionId == sectionId && !t.Isdeleted && t.Tablestatus == 0)
    //     .ToList();

    //     return tables;
    // }

    #endregion

    #region Menu

    public async Task<CategoryItemsForOrderMenuViewModel> GetCategoryItemsForOrderMenu(string? searchText, int categoryId, int? orderId)
    {
        List<Item>? items = _orderAppRepository.GetFilteredItems(categoryId, searchText);
        List<Menucategory>? categories = _orderAppRepository.GetAllCategories();
        string? SectionName = _orderAppRepository.GetSectionName(orderId);
        List<string>? TableNames = _orderAppRepository.GetTableNames(orderId);
        List<Taxesandfee>? taxes = _orderAppRepository.GetEnabledTaxes();

        CategoryItemsForOrderMenuViewModel? model = new()
        {
            Categories = categories,
            Items = items,
            SelectedCategoryId = categoryId,
            ActiveOrderId = orderId,
            SectionName = SectionName,
            TableName = TableNames,
            Taxes = taxes
        };
        return model;
    }

    public void ToggleFavorite(int itemId)
    {
        _orderAppRepository.ToggleFavorite(itemId);
    }

    public async Task<ModifierSelectionModalViewModel?> GetModifiersGroupedByItemAsync(int itemId)
    {
        return await _orderAppRepository.GetModifiersGroupedByItemAsync(itemId);
    }

    public async Task<OrderItemForRowViewModel?> RenderOrderItemRow(RenderOrderItemRowRequest request)
    {
        Console.WriteLine($"ItemId: {request.ItemId}, ItemName: {request.ItemName}, BasePrice: {request.BasePrice}, Quantity: {request.Quantity}, Index: {request.Index}, Instruction: {request.Instruction}, SelectedModifiers: {request.SelectedModifiers}");
        int? readyQty = await _orderAppRepository.GetReadyQuantity(request.ItemId, request.OrderId);

        OrderItemForRowViewModel? viewModel = new()
        {
            OrderId = request.OrderId,
            ItemId = request.ItemId,
            Index = request.Index,
            ItemName = request.ItemName,
            Instruction = request.Instruction ?? "",
            Rate = request.BasePrice,
            MaxQuantity = request.MaxQuantity,
            Quantity = readyQty ?? 0,
            OrderedQuantity = request.Quantity,
            SelectedModifiers = request.SelectedModifiers
        };
        return viewModel;
    }

    public async Task<OrderCustomerViewModel?> GetOrderCustomerAsync(int orderId)
    {
        return await _orderAppRepository.GetOrderCustomerAsync(orderId);
    }

    public async Task<bool> UpdateCustomerAsync(UpdateCustomerViewModel model)
    {
        return await _orderAppRepository.UpdateCustomerAsync(model);
    }

    public async Task<string?> GetAdminCommentAsync(int id)
    {
        return await _orderAppRepository.GetAdminCommentAsync(id);
    }

    public async Task<bool> SaveAdminCommentAsync(int orderDetailId, string adminComment)
    {
        return await _orderAppRepository.SaveAdminCommentAsync(orderDetailId, adminComment);
    }

    public async Task<string?> GetItemInstructionAsync(int orderId, int itemId)
    {
        return await _orderAppRepository.GetItemInstructionAsync(orderId, itemId);
    }

    public async Task<bool> SaveItemInstructionAsync(int orderId, int itemId, string instruction)
    {
        return await _orderAppRepository.SaveItemInstructionAsync(orderId, itemId, instruction);
    }

    public async Task<List<SaveOrderDetailViewModel>?> GetOrderDetailsById(int orderId)
    {
        Order? order = await _orderAppRepository.GetOrderByIdAsync(orderId);

        // Project order details to DTO
        List<SaveOrderDetailViewModel>? orderItems = order.Orderdetails.Select(od => new SaveOrderDetailViewModel
        {
            Id = (int)od.Orderid,
            ItemId = (int)od.Itemid,
            ItemName = od.Item.Itemname,
            Quantity = od.Quantity,
            MaxQuantity = Convert.ToInt32(od.Item.Quantity),
            Rate = od.Price,
            Modifiers = od.Ordermodifierdetails.Select(m => new SaveModifierViewModel
            {
                ModifierId = m.Itemid ?? 0,
                ModifierName = _orderAppRepository.GetModifierNameById(m.Itemid),
                Rate = m.Price
            }).ToList()
        }).ToList();

        return orderItems;
    }

    public async Task<List<SaveOrderDetailViewModel>?> SaveOrderItemsAsync(SaveOrderRequestViewModel model)
    {
        List<Orderdetail>? updatedOrderDetails = await _orderAppRepository.SaveOrderItemsAsync(model);

        List<SaveOrderDetailViewModel>? orderDetailDtos = updatedOrderDetails.Select(od => new SaveOrderDetailViewModel
        {
            Id = (int)od.Orderid,
            ItemId = (int)od.Itemid,
            ItemName = od.Item.Itemname ?? "",
            Quantity = od.Quantity,
            AvailableQuantity = od.Availablequantity,
            Rate = od.Price,
            Modifiers = od.Ordermodifierdetails.Select(m => new SaveModifierViewModel
            {
                ModifierId = (int)m.Itemid,
                ModifierName = m.Item.Itemname ?? "",
                Rate = m.Price
            }).ToList()
        }).ToList();

        return orderDetailDtos;
    }

    public async Task<List<SaveOrderDetailViewModel>?> CompleteOrderAsync(int orderId)
    {
        List<SaveOrderDetailViewModel>? updatedOrderDetails = await _orderAppRepository.CompleteOrderItemsAsync(orderId);
        return updatedOrderDetails;
    }

    public async Task<List<SaveOrderDetailViewModel>?> CancelOrderAsync(int orderId)
    {
        List<SaveOrderDetailViewModel>? updatedOrderDetails = await _orderAppRepository.CancelOrderItemsAsync(orderId);
        return updatedOrderDetails;
    }

    #endregion

}