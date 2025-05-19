using PizzaShop.Entity.Models;

namespace PizzaShop.Service.Interfaces;

public interface ILocationService
{
    List<Country> GetCountries();
    List<State> GetStates(int countryId);
    List<City> GetCities(int stateId);
}
