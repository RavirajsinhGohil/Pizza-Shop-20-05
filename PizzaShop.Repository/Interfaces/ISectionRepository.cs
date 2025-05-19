using PizzaShop.Repository.GetDataFromToken;
using PizzaShop.Entity.ViewModel;
using PizzaShop.Entity.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ISectionRepository
{
    Task<SectionNameViewModel> GetSectionById(int sectionId);
    IEnumerable<SectionNameViewModel> GetSectionList();
    Task<List<Section>> GetAllSections();
    Task<List<Tablegrouping>> GetAssignedTables();
    Task<List<Table>> GetTablesBySectionId(int sectionId);
    TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber = 1, int pageSize = 2, string searchKeyword = "");
    Task<AuthResponse> AddSection(AddSectionViewModel model);
    Task<AuthResponse> EditSection(AddSectionViewModel model);
    Task<AuthResponse> DeleteSection(int id);
    Task<AuthResponse> AddTable(AddTableViewmodel model);
    Task<AuthResponse> EditTable(AddTableViewmodel model);
    Task<AuthResponse> DeleteTable(int id);
    Task<AuthResponse> DeleteTables(List<int> ids);

}