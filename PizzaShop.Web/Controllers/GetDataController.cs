using Microsoft.AspNetCore.Mvc;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Web.Controllers;

[Route("GetData")]
public class GetDataController : Controller
{
    private readonly ILocationService _locationService;

        public GetDataController(ILocationService regionService)
        {
            _locationService = regionService;
        }

        [HttpGet("GetCountries")]
        public IActionResult GetCountries()
        {
            var countries = _locationService.GetCountries()
                .Select(c => new
                {
                    countryId = c.Countryid,
                    countryName = c.Countryname
                })
                .ToList();

            return Ok(countries);
        }

        [HttpGet("GetStates")]
        public IActionResult GetStates(int countryId)
        {
            var states = _locationService.GetStates(countryId)
                .Select(s => new
                {
                    stateId = s.Stateid,
                    stateName = s.Statename
                })
                .ToList();

            return Ok(states);
        }

        [HttpGet("GetCities")]
        public IActionResult GetCities(int stateId)
        {
            var cities = _locationService.GetCities(stateId)
                .Select(c => new
                {
                    cityId = c.Cityid,
                    cityName = c.Cityname
                })
                .ToList();

            return Ok(cities);
        }
}
