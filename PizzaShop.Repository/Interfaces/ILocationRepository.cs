using PizzaShop.Entity.Models;

namespace PizzaShop.Repository.Interfaces;

public interface ILocationRepository
{
    List<Country> GetCountries();
    List<State> GetStates(int countryId);
    List<City> GetCities(int stateId);
}
