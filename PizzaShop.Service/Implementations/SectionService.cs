using PizzaShop.Entity.ViewModel;
using PizzaShop.Service.Interfaces;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.GetDataFromToken;
using PizzaShop.Entity.Models;

namespace PizzaShop.Service.Implementations;

public class SectionService : ISectionService
{
    private readonly ISectionRepository _sectionRepository;
    public SectionService(ISectionRepository sectionRepository)
    {
        _sectionRepository = sectionRepository;
    }

    public async Task<SectionNameViewModel> GetSectionById(int sectionId)
    {
        return await _sectionRepository.GetSectionById(sectionId);
    }

    public IEnumerable<SectionNameViewModel> GetSectionList()
    {
        return _sectionRepository.GetSectionList();
    }

    public async Task<List<OrderAppSectionViewModel>> GetAllSections()
    {
        List<Section> sections = await _sectionRepository.GetAllSections();
        List<Tablegrouping>? AssignedTables = await _sectionRepository.GetAssignedTables();
        List<Waitingticket>? Waitingtickets = sections.SelectMany(s => s.Waitingtickets).ToList();
        List<OrderAppSectionViewModel> appSection = new();
        foreach (var section in sections)
        {
            List<TableCard> tableCards = new();
            foreach (var table in section.Tables)
            {
                TableCard tableCard = new()
                {
                    TableId = table.Tableid,
                    TableName = table.Tablename,
                    AssignedTime = table.Assignedtime,
                    Capacity = table.Capacity,
                    TableStatus = table.NewstatusNavigation?.Statusname,
                    OrderId = AssignedTables.Where(t => t.Tableid == table.Tableid).Select(t => t.Orderid).FirstOrDefault(),
                };
                tableCards.Add(tableCard);
            }
            OrderAppSectionViewModel model = new()
            {
                SectionId = section.Sectionid,
                SectionName = section.Sectionname,
                Tables = tableCards,
                Available = tableCards.Count(t => t.TableStatus == "Available"),
                Assigned = tableCards.Count(t => t.TableStatus == "Assigned"),
                Running = tableCards.Count(t => t.TableStatus == "Running"),
            };
            appSection.Add(model);
        }
        return appSection;
    }

    public async Task<List<TableViewModel>> GetTablesBySectionId(int sectionId)
    {
        List<Table> tables = await _sectionRepository.GetTablesBySectionId(sectionId);
        List<TableViewModel> tablesModel = new();
        foreach (var table in tables)
        {
            TableViewModel tableViewModel = new()
            {
                TableId = table.Tableid,
                TableName = table.Tablename,
                SectionId = table.Sectionid,
                SectionName = table.Section.Sectionname,
                Capacity = table.Capacity,
                Status = table.Status
            };
            tablesModel.Add(tableViewModel);
        }
        return tablesModel;
    }

    public TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber, int pageSize, string searchKeyword)
    {
        return _sectionRepository.GetDiningTablesListBySectionId(sectionid, pageNumber, pageSize, searchKeyword);
    }

    public async Task<AuthResponse> AddSection(AddSectionViewModel model)
    {
        return await _sectionRepository.AddSection(model);
    }

    public async Task<AuthResponse> EditSection(AddSectionViewModel model)
    {
        return await _sectionRepository.EditSection(model);
    }

    public async Task<AuthResponse> DeleteSection(int id)
    {
        return await _sectionRepository.DeleteSection(id);
    }

    public async Task<AuthResponse> AddTable(AddTableViewmodel model)
    {
        return await _sectionRepository.AddTable(model);
    }

    public async Task<AuthResponse> EditTable(AddTableViewmodel model)
    {
        return await _sectionRepository.EditTable(model);
    }

    public async Task<AuthResponse> DeleteTable(int id)
    {
        return await _sectionRepository.DeleteTable(id);
    }

    public async Task<AuthResponse> DeleteTables(List<int> ids)
    {
        return await _sectionRepository.DeleteTables(ids);
    }
}
