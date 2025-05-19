

using PizzaShop.Repository.Helper;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;
using PizzaShop.Entity.Data;
using PizzaShop.Entity.ViewModel;
using Microsoft.AspNetCore.Http;
using PizzaShop.Repository.GetDataFromToken;

namespace PizzaShop.Repository.Implementations;

public class TaxRepository:ITaxRepository
{   
    private readonly ApplicationDbContext _dbo;

    private readonly IHttpContextAccessor _httpContext;

    // private readonly ITokenRepository _jwtservices;

    // private readonly IuserService _userservices;

    public TaxRepository(ApplicationDbContext dbo, IHttpContextAccessor httpContext, IUserRepository userService)
    {
        _dbo = dbo;
        _httpContext = httpContext;
        // _userservices = userService;
    }

    public TaxListPaginationViewModel GetTaxList(int pageNumber = 1, int pageSize = 2, string searchKeyword = "")
    {
        TaxListPaginationViewModel model = new() { Page = new() };
        searchKeyword = searchKeyword.ToLower();

        var query = from i in _dbo.Taxesandfees
                    where i.Isdeleted != true
                    select new TaxViewModel
                    {   
                        TaxId = i.Taxid,
                        TaxName = i.Taxname,
                        Type = i.Taxtype,
                        TaxAmount = i.Taxvalue,
                        Isenable = i.Isenabled,
                        Isdefault = i.Isdefault
                    };

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(i => i.TaxName.ToLower().Contains(searchKeyword));
        }

        // Pagination
        int totalCount = query.Count();
        query = query.OrderBy(i => i.TaxName);
        var items = query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

        model.Items = items;
        model.Page.SetPagination(totalCount, pageSize, pageNumber);
        return model;
    }


    // Add Tax
    public async Task<AuthResponse> AddTax(AddTaxViewModel model)
    {   
        try{
        var token = _httpContext.HttpContext.Request.Cookies["Token"];
        // var userid = _userservices.(token);

        var tax = new Taxesandfee
        {
            Taxname = model.TaxName,
            Taxtype = model.Type,
            Isenabled = model.Isenable,
            Isdefault = model.Isdefault,
            Taxvalue = model.TaxAmount,
            // Createdby = userid
        };

        _dbo.Taxesandfees.Add(tax);
        await _dbo.SaveChangesAsync();

        return new AuthResponse
            {
                Success = true,
                Message = "Tax Added Succesfully!"
            };
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error in Add Tax: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Adding Tax!"
            };
        }
    }


    // Edit Tax
    public async Task<AuthResponse> EditTax(AddTaxViewModel model)
    {   
        try{
        var token = _httpContext.HttpContext.Request.Cookies["Token"];
        // var userid = _userservices.GetUserIdfromToken(token);

        var existingtax = _dbo.Taxesandfees.FirstOrDefault(i=> i.Taxid==model.TaxId);

        
            // existingtax.Taxname = model.TaxName;
            // existingtax.Taxtype = model.Type;
            // existingtax.Isenabled = model.Isenable;
            // existingtax.Isdefault = model.Isdefault;
            existingtax.Taxvalue = model.TaxAmount;
            // existingtax.Updatedby = userid;
        

        _dbo.Taxesandfees.Update(existingtax);
        await _dbo.SaveChangesAsync();

        return new AuthResponse
            {
                Success = true,
                Message = "Tax Updated Succesfully!"
            };
        }
        catch(Exception e)
        {
            Console.WriteLine($"Error in Edit Tax: {e.Message}");

            return new AuthResponse
            {
                Success = false,
                Message = "Error in Updating Tax!"
            };
        }
    }


    // Delete single Tax
    public async Task<AuthResponse> DeleteTax(int id)
    {
        var item = _dbo.Taxesandfees.FirstOrDefault(c => c.Taxid == id);

        if (item != null)
        {
            // item.Isdeleted = true;

            await _dbo.SaveChangesAsync();

            return new AuthResponse
            {
                Success = true,
                Message = "Tax Deleted Successfully"
            };
        }
        else
        {
            return new AuthResponse
            {
                Success = false,
                Message = "can't delete Tax"

            };
        }
    }

}