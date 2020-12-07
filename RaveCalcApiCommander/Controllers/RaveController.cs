﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using ConvertGeoNamesDBToMongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RaveCalcApiCommander.Data;
using RaveCalcApiCommander.Models;
using Stratogos.Jovian.Rave.Charts;
using Stratogos.Jovian.Rave.Structures;
using TimeZoneCorrectorLibrary.Abstraction;

namespace RaveCalcApiCommander.Controllers
{
    [Authorize]
    [Route("apiv2")]
    [ApiController]
    public class RaveController : ControllerBase
    {
        private readonly ILogger<RaveController> _logger;
        private readonly IRaveRepository _raveRepo;
        private readonly IConfiguration _config;
        private readonly ITimeZoneCorrector _timeZoneCorrector;

        public RaveController(ILogger<RaveController> logger, IRaveRepository raveRepo, IConfiguration config,
            ITimeZoneCorrector timeZoneCorrector)
        {
            _logger = logger;
            _raveRepo = raveRepo;
            _config = config;
            _timeZoneCorrector = timeZoneCorrector;
        }

        [HttpGet]
        [Route("getallcountries")]
        public ActionResult GetAllCountriesName()
        {
            var countries = _timeZoneCorrector.GetAllCountries();
            if(countries == null)
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
        [Route("getallstates")]
        public ActionResult GetAllStatesByCountryName([FromQuery] StatesQuery query)
        {
            if(query.countryName == null)
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
            var states = _timeZoneCorrector.GetAllStateByCountry(query.countryName);
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
        [Route("getallcities")]
        public ActionResult GetAllCitiesByCountryAndStateName([FromQuery] CitiesQuery query)
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
            var cities = _timeZoneCorrector.GetAllCitiesByState(query.countryName, query.stateName);
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
        public ActionResult GetCityByCountryStateAndCityName([FromQuery] CityQuery query)
        {
            if (query.countryName == null)
            {
                ModelState.AddModelError("countryName", "Отсутствует значение параметра");
            }
            if(query.cityName == null)
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
            var city = _timeZoneCorrector.GetCityByCountryAndState(query.countryName, query.stateName, 
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

        [HttpGet]
        [Route("utcdatabytz")]
        public ActionResult GetUTCDataTimeByTimeZone([FromQuery] DataQuery query)
        {
            if (query.timeZoneName == null)
            {
                ModelState.AddModelError("timeZoneName", "Отсутствует значение параметра");
            }
            if (query.date == null)
            {
                ModelState.AddModelError("date", "Отсутствует значение параметра");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }

            DateTime dateFormat;
            if (!DateTime.TryParse(query.date, null, System.Globalization.DateTimeStyles.RoundtripKind, out dateFormat))
            {
                ModelState.AddModelError("date", "Недопустимый формат даты");
            }
            try
            {
                var utcDate = _timeZoneCorrector.ConvertToUtcFromCustomTimeZone(query.timeZoneName, dateFormat);
                return Ok(new ResponseResult<DateTime>()
                {
                    error = false,
                    result = utcDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "Date not converted"
                });
            }
        }

        //2020-03-05T12:01:41
        [HttpGet]
        [Route("rave-chart")]
        public ActionResult GetRaveChartJson([FromQuery]Query query)
        {
            if(query.birthdate == null)
            {
                ModelState.AddModelError("birthdate", "Отсутствует значение параметра");
            }
            DateTime birthDate;
            if (!DateTime.TryParse(query.birthdate, null, System.Globalization.DateTimeStyles.RoundtripKind, out birthDate))
            {
                ModelState.AddModelError("birthdate", "Недопустимый формат даты");
            }
            else if (birthDate.Year < 1200)
            {
                ModelState.AddModelError("birthdate", "Год должен быть больше 1200");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var rave =_raveRepo.GetRaveChartInJson(birthDate) as AdvancedImagingChart;
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
            });

        }


        [HttpGet]
        [Route("rave-circlechart")]
        public ActionResult GetRaveCircleChartJson([FromQuery]CycleQuery query)
        {
            if (query.birthdate == null)
            {
                ModelState.AddModelError("birthdate", "Отсутствует значение параметра");
            }
            if (query.cycledate == null)
            {
                ModelState.AddModelError("cycledate", "Отсутствует значение параметра");
            }

            if (!DateTime.TryParse(query.birthdate, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime birthDate))
            {
                ModelState.AddModelError("birthdate", "Недопустимый формат даты");
            }
            else if (birthDate.Year < 1200)
            {
                ModelState.AddModelError("birthdate", "Год должен быть больше 1200");
            }

            if (!DateTime.TryParse(query.cycledate, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime cycleDate))
            {
                ModelState.AddModelError("cycledate", "Недопустимый формат даты");
            }
            else if (cycleDate.Year < 1200)
            {
                ModelState.AddModelError("cycleDate", "Год должен быть больше 1200");
            }

            if (!Enum.TryParse(query.cycletype, out HdStructures.Cycle cycleType))
            {
                ModelState.AddModelError("cycletype", "Недопустимый формат типа цикла");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var rave = _raveRepo.GetCycleChartInJson(birthDate, cycleDate, cycleType );
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
            });
        }

        [HttpGet]
        [Route("rave-transitchart")]
        public ActionResult GetRaveTransitChartJson([FromQuery]TransitQuery query)
        {
            if (query.birthdate == null)
            {
                ModelState.AddModelError("birthdate", "Отсутствует значение параметра");
            }
            if (query.transitdate == null)
            {
                ModelState.AddModelError("transitdate", "Отсутствует значение параметра");
            }

            if (!DateTime.TryParse(query.birthdate, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime birthDate))
            {
                ModelState.AddModelError("birthdate", "Недопустимый формат даты");
            }
            else if (birthDate.Year < 1200)
            {
                ModelState.AddModelError("birthdate", "Год должен быть больше 1200");
            }

            if (!DateTime.TryParse(query.transitdate, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime transitDate))
            {
                ModelState.AddModelError("transitdate", "Недопустимый формат даты");
            }
            else if (transitDate.Year < 1200)
            {
                ModelState.AddModelError("transitDate", "Год должен быть больше 1200");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var rave = _raveRepo.GetTransitChartInJson(birthDate, transitDate);
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
            });
        }

        [HttpGet]
        [Route("rave-connectionchart")]
        public ActionResult GetRaveConnectionChartJson([FromQuery]ConnectionQuery query)
        {
            if (query.birthdate1 == null)
            {
                ModelState.AddModelError("birthdate1", "Отсутствует значение параметра");
            }
            if (query.birthdate2 == null)
            {
                ModelState.AddModelError("birthdate2", "Отсутствует значение параметра");
            }

            if (!DateTime.TryParse(query.birthdate1, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime birthDate1))
            {
                ModelState.AddModelError("birthdate1", "Недопустимый формат даты");
            }
            else if (birthDate1.Year < 1200)
            {
                ModelState.AddModelError("birthdate1", "Год должен быть больше 1200");
            }

            if (!DateTime.TryParse(query.birthdate2, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime birthDate2))
            {
                ModelState.AddModelError("birthdate2", "Недопустимый формат даты");
            }
            else if (birthDate2.Year < 1200)
            {
                ModelState.AddModelError("birthdate1", "Год должен быть больше 1200");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var rave = _raveRepo.GetConnectionChartInJson(birthDate1, birthDate2);
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
            });
        }

        [HttpGet]
        [Route("rave-pentamodel")]
        public ActionResult GetRavePentaModelJson([FromQuery]PentaQuery query)
        {
            ArrayList birthdates = new ArrayList();
            DateTime birthDate;
            if (query.birthdate1 == null)
            {
                ModelState.AddModelError("birthdate1", "Отсутствует значение параметра");
            }
            if (query.birthdate2 == null)
            {
                ModelState.AddModelError("birthdate2", "Отсутствует значение параметра");
            }
            if (query.birthdate3 == null)
            {
                ModelState.AddModelError("birthdate3", "Отсутствует значение параметра");
            }

            if (query.birthdate1 != null)
            {
                if (!DateTime.TryParse(query.birthdate1, null, System.Globalization.DateTimeStyles.RoundtripKind, out birthDate))
                {
                    ModelState.AddModelError("birthdate1", "Недопустимый формат даты");
                }
                else if (birthDate.Year < 1200)
                {
                    ModelState.AddModelError("birthdate1", "Год должен быть больше 1200");
                }
                birthdates.Add(birthDate);
            }

            if (query.birthdate2 != null)
            {
                if (!DateTime.TryParse(query.birthdate2, null, System.Globalization.DateTimeStyles.RoundtripKind, out birthDate))
                {
                    ModelState.AddModelError("birthdate2", "Недопустимый формат даты");
                }
                else if (birthDate.Year < 1200)
                {
                    ModelState.AddModelError("birthdate2", "Год должен быть больше 1200");
                }

                birthdates.Add(birthDate);
            }
            if (query.birthdate3 != null)
            {
                if (!DateTime.TryParse(query.birthdate3, null, System.Globalization.DateTimeStyles.RoundtripKind, out birthDate))
                {
                    ModelState.AddModelError("birthdate3", "Недопустимый формат даты");
                }
                else if (birthDate.Year < 1200)
                {
                    ModelState.AddModelError("birthdate3", "Год должен быть больше 1200");
                }

                birthdates.Add(birthDate);
            }
            if (query.birthdate4 != null)
            {
                if (!DateTime.TryParse(query.birthdate4, null, System.Globalization.DateTimeStyles.RoundtripKind, out birthDate))
                {
                    ModelState.AddModelError("birthdate4", "Недопустимый формат даты");
                }
                else if (birthDate.Year < 1200)
                {
                    ModelState.AddModelError("birthdate4", "Год должен быть больше 1200");
                }

                birthdates.Add(birthDate);
            }
            if (query.birthdate5 != null)
            {
                if (!DateTime.TryParse(query.birthdate5, null, System.Globalization.DateTimeStyles.RoundtripKind, out birthDate))
                {
                    ModelState.AddModelError("birthdate5", "Недопустимый формат даты");
                }
                else if (birthDate.Year < 1200)
                {
                    ModelState.AddModelError("birthdate5", "Год должен быть больше 1200");
                }

                birthdates.Add(birthDate);
            }

            if (birthdates.Count < 3)
            {
                ModelState.AddModelError("birthdates", "Должно быть минимум три даты рождения");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            var rave = _raveRepo.GetPentaModelInJson(birthdates);
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
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