using PizzaShop.Entity.Models;
using PizzaShop.Repository.GetDataFromToken;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Helper;
using PizzaShop.Entity.ViewModel;
using Microsoft.AspNetCore.Http;
using PizzaShop.Entity.Data;
using Microsoft.EntityFrameworkCore;

namespace PizzaShop.Repository.Implementations;


public class SectionRepository : ISectionRepository
{
    private readonly ApplicationDbContext _dbo;

    private readonly IHttpContextAccessor _httpContext;


    private readonly IUserRepository _userservices;

    public SectionRepository(ApplicationDbContext dbo, IHttpContextAccessor httpContext, IUserRepository userservices)
    {
        _dbo = dbo;
        _httpContext = httpContext;
        _userservices = userservices;
    }

    public async Task<SectionNameViewModel> GetSectionById(int sectionId)
    {
        return await _dbo.Sections
                    .Where(s => s.Sectionid == sectionId)
                    .Select(s => new SectionNameViewModel
                    {
                        SectionId = s.Sectionid,
                        SectionName = s.Sectionname,
                        Description = s.Description
                    }).FirstAsync();
    }

    // Get List Of All Sections
    public IEnumerable<SectionNameViewModel> GetSectionList()
    {
        var sections = _dbo.Sections
                        .Where(c => c.Isdeleted != true)
                        .OrderBy(c => c.Sectionid)
                        .Select(c => new SectionNameViewModel
                        {
                           SectionId = c.Sectionid,
                           SectionName = c.Sectionname,
                           Description = c.Description
                        }).ToList();

        sections = new List<SectionNameViewModel>(sections);
        return sections;
    }

    public async Task<List<Section>> GetAllSections()
    {
        return await _dbo.Sections.Where(c => c.Isdeleted != true).Include(c => c.Waitingtickets)
                    .Include(c => c.Tables).ThenInclude(s => s.NewstatusNavigation)
                    .ToListAsync();
    }

    public async Task<List<Tablegrouping>> GetAssignedTables()
    {
        return await _dbo.Tablegroupings.Where(t => t.Isdeleted == false).Include(t => t.Table).ToListAsync();
    }

    public async Task<List<Table>> GetTablesBySectionId(int sectionId)
    {
        return await _dbo.Tables.Where(t => t.Sectionid == sectionId).ToListAsync();
    }

    // Return list Of Pagination Table List

    public TableListPaginationViewModel GetDiningTablesListBySectionId(int sectionid, int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {

        TableListPaginationViewModel model = new() { Page = new() };
        searchKeyword = searchKeyword.ToLower();
        // var categoryId = _dbo.Categories.FirstOrDefault(c => c.CategoryId == categoryid)?.CategoryId;

        var query = from i in _dbo.Tables
                    where i.Isdeleted != true && i.Sectionid == sectionid
                    select new TableViewModel
                    {
                        TableId = i.Tableid,
                        SectionId = i.Sectionid,
                        Name = i.Tablename,
                        Capacity = i.Capacity,
                        Status = i.Status
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.Name.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        query = query.OrderBy(i => i.Name);
        List<TableViewModel>? items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.Sectionid = sectionid;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
        return model;
    }

    // Add new Section
    public async Task<AuthResponse> AddSection(AddSectionViewModel model)
    {
        try
        {
            Section? sectionCheck = _dbo.Sections.FirstOrDefault(s => s.Sectionname == model.SectionName);
            if (sectionCheck != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Section Already Exists!"
                };
            }
            string? token = _httpContext.HttpContext.Request.Cookies["Token"];
            // var userid = _userservices.GetUserIdfromToken(token);

            Section? section = new()
            {
                Sectionname = model.SectionName,
                Description = model.Description,
                // Createdby = userid
            };

            _dbo.Sections.Add(section);
            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Section Added Succesfully!"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in AddSection: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Adding Section!"
            };
        }
    }

    // Edit Section
    public async Task<AuthResponse> EditSection(AddSectionViewModel model)
    {
        try
        {
            string? token = _httpContext.HttpContext.Request.Cookies["Token"];
            // var userid = _userservices.GetUserIdfromToken(token);


            var existingsection = _dbo.Sections.FirstOrDefault(s => s.Sectionid == model.Sectionid);

            if (existingsection == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Section not found!"
                };
            }

            existingsection.Sectionname = model.SectionName;
            existingsection.Description = model.Description;
            // existingsection.Updatedby = userid;

            _dbo.Sections.Update(existingsection);
            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Section Updated Succesfully!"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in Updating Section!");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Updating Section!"
            };
        }
    }

    // Delete section 
    public async Task<AuthResponse> DeleteSection(int id)
    {
        var section = _dbo.Sections.FirstOrDefault(c => c.Sectionid == id);

        if (section != null)
        {
            // Fetch all tables that belong to this category
            var items = _dbo.Tables.Where(i => i.Sectionid == id).ToList();

            // Mark all items as deleted
            foreach (var item in items)
            {
                item.Isdeleted = true;
            }

            section.Isdeleted = true;

            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "section deleted successfully!"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Error in deleting Section!"

            };
        }
    }


    // Add New Table
    public async Task<AuthResponse> AddTable(AddTableViewmodel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["Token"];

            var table = new Table
            {
                Sectionid = model.SectionId,
                Tablename = model.Name,
                Capacity = model.Capacity ?? 0,
                Status= true, // true => Available (When Table is created)
                Newstatus = 1,  //  1=> Available, 2 = Assigned, 3 = Running
                Isdeleted = false
                // Createdby = userid
            };

            _dbo.Tables.Add(table);
            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Table Added Succesfully!"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in AddTable: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Adding Table!"
            };
        }
    }

    //  Edit Table 
    public async Task<AuthResponse> EditTable(AddTableViewmodel model)
    {
        try
        {
            var token = _httpContext.HttpContext.Request.Cookies["Token"];
            // var userid = _userservices.GetUserIdfromToken(token);

            var table = _dbo.Tables.FirstOrDefault(t => t.Tableid == model.TableId);

            table.Sectionid = model.SectionId;
            table.Tablename = model.Name;
            table.Capacity = model.Capacity ?? 0;
            // table.Updatedby = userid;


            _dbo.Tables.Update(table);
            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Table Updated Succesfully!"
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in UpdateTable: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Updating Table!"
            };
        }
    }

    // Delete single Table
    public async Task<AuthResponse> DeleteTable(int id)
    {
        var item = _dbo.Tables.FirstOrDefault(c => c.Tableid == id);

        if (item != null)
        {
            item.Isdeleted = true;

            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Table Deleted Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "can't delete table"

            };
        }
    }

    // Delete Multiple Tables

    public async Task<AuthResponse> DeleteTables(List<int> ids)
    {
        try
        {
            foreach (var i in ids)
            {
                var item = _dbo.Tables.FirstOrDefault(itemInDb => itemInDb.Tableid == i);
                item.Isdeleted = true;
                _dbo.Tables.Update(item);
            }
            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Tables Deleted Succesfully!"
            };
        }
        catch (Exception)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "error in Delete Tables!"
            };
            throw;
        }

    }

}