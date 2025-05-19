using PizzaShop.Entity.Data;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Entity.Models;

namespace PizzaShop.Repository.Implementations;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _dbo;

        public LocationRepository(ApplicationDbContext dbo)
        {
            _dbo = dbo;
        }

        public List<Country> GetCountries()
        {
            return _dbo.Countries.ToList();
        }

        public List<State> GetStates(int countryId)
        {
            return _dbo.States.Where(s => s.Countryid == countryId).ToList();
        }

        public List<City> GetCities(int stateId)
        {
            return _dbo.Cities.Where(c => c.Stateid == stateId).ToList();
        }
}
