using ConvertGeoNamesDBToMongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RaveCalcApiCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneCorrectorLibrary.Abstraction;

namespace RaveCalcApiCommander
{
    [Route("apiv2")]
    [ApiController]
    public class CityInfoController : ControllerBase
    {
        private readonly ILogger<CityInfoController> _logger;
        private readonly IConfiguration _config;
        private readonly ITimeZoneCorrector _timeZoneCorrector;

        public CityInfoController(ILogger<CityInfoController> logger, IConfiguration config,
            ITimeZoneCorrector timeZoneCorrector)
        {
            _logger = logger;
            _config = config;
            _timeZoneCorrector = timeZoneCorrector;
        }

        [HttpGet]
        [Route("getallcountries")]
        public ActionResult GetAllCountriesName()
        {
            var countries = _timeZoneCorrector.GetCountries();
            if (countries == null)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "Countries not found"
                });
            }
            return Ok(new ResponseResult<List<string>>()
            {
                error = false,
                result = countries
            });
        }

        [HttpGet]
        [Route("getcountryinfo")]
        public async Task<ActionResult> GetCountryInfo([FromQuery] StatesQuery query)
        {
            if (query.countryName == null)
            {
                ModelState.AddModelError("countryName", "Отсутствует значение параметра");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var country = await _timeZoneCorrector.GetCountryInfo(query.countryName);
            if (country == null)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "Country not found"
                });
            }
            return Ok(new ResponseResult<Country>()
            {
                error = false,
                result = country
            });
        }

        [Authorize]
        [HttpGet]
        [Route("getallstates")]
        public async Task<ActionResult> GetAllStates([FromQuery] StatesQuery query)
        {
            if (query.countryName == null)
            {
                ModelState.AddModelError("countryName", "Отсутствует значение параметра");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var states = await _timeZoneCorrector.GetStates(query.countryName);
            if (states == null)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "States not found"
                });
            }
            return Ok(new ResponseResult<List<string>>()
            {
                error = false,
                result = states
            });
        }

        [HttpGet]
        [Route("getalldistricts")]
        public async Task<ActionResult> GetAllDistrict([FromQuery] DistrictsQuery query)
        {
            if (query.countryName == null)
            {
                ModelState.AddModelError("countryName", "Отсутствует значение параметра");
            }
            if (query.stateName == null)
            {
                ModelState.AddModelError("stateName", "Отсутствует значение параметра");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var states = await _timeZoneCorrector.GetDistricts(query.countryName, query.stateName);
            if (states == null)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "Districts not found"
                });
            }
            return Ok(new ResponseResult<List<string>>()
            {
                error = false,
                result = states
            });
        }

        [HttpGet]
        [Route("getallcities")]
        public async Task<ActionResult> GetAllCities([FromQuery] CitiesQuery query)
        {
            if (query.countryName == null)
            {
                ModelState.AddModelError("countryName", "Отсутствует значение параметра");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var cities = await _timeZoneCorrector.GetCities(query.countryName, query.stateName, query.districtName);
            if (cities == null)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "Cities not found"
                });
            }
            return Ok(new ResponseResult<List<string>>()
            {
                error = false,
                result = cities
            });
        }

        [HttpGet]
        [Route("getcity")]
        public async Task<ActionResult> GetCity([FromQuery] CityQuery query)
        {
            if (query.countryName == null)
            {
                ModelState.AddModelError("countryName", "Отсутствует значение параметра");
            }
            if (query.cityName == null)
            {
                ModelState.AddModelError("cityName", "Отсутствует значение параметра");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var city = await _timeZoneCorrector.GetCity(query.countryName, query.stateName, query.districtName,
                query.cityName);
            if (city == null)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "City not found"
                });
            }

            return Ok(new ResponseResult<City>()
            {
                error = false,
                result = city
            });
        }

        public string GetModelStateError()
        {
            var query = (from kvp in ModelState
                         let field = kvp.Key
                         let state = kvp.Value
                         where state.Errors.Count > 0
                         let val = state.AttemptedValue ?? "[NULL]"
                         let errors = string.Join(";", state.Errors.Select(err => err.ErrorMessage))
                         select string.Format("{0}:[{1}] (ERRORS: {2})", field, val, errors));
            return string.Join("  ", query);
        }
    }
}
