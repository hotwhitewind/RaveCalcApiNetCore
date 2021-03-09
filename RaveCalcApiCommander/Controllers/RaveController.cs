using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConvertGeoNamesDBToMongoDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RaveCalcApiCommander.Abstraction;
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

        //2020-03-05T12:01:41
        [HttpGet]
        [Route("rave-chart")]
        public async Task<ActionResult> GetRaveChartJson([FromQuery]Query query)
        {
            if(query.birthdate == null)
            {
                ModelState.AddModelError("birthdate", "Отсутствует значение параметра");
            }

            if(!string.IsNullOrEmpty(query.timezone) && query.city != null)
            {
                ModelState.AddModelError("timezone", "Необходимо указать только один параметр: город или timezone");
            }

            CheckDateParam(query.birthdate, "birthdate", out DateTime birthDate);
            CheckCityParam(query.city, null);
            DateTime queryBirthDate = birthDate;

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            if (query.city != null || !string.IsNullOrEmpty(query.timezone))
            {
                DateConvertResult res;
                if (!string.IsNullOrEmpty(query.timezone))                
                    res = ConvertDate(query.timezone, birthDate);
                else
                    res = await ConvertDate(query.city, birthDate);
                birthDate = res.dateConvertOut;
                if (res.errorType == -1)
                {
                    return BadRequest(new ResponseError
                    {
                        error = true,
                        message = "City not found"
                    });
                }
                if (res.errorType == -2)
                {
                    return BadRequest(new ResponseError
                    {
                        error = true,
                        message = "Date not converted"
                    });
                }
            }
            var rave =_raveRepo.GetRaveChartInJson(birthDate) as AdvancedImagingChart;
            rave.BirthDate = queryBirthDate;
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
            });
        }

        [HttpGet]
        [Route("rave-circlechart")]
        public async Task<ActionResult> GetRaveCircleChartJson([FromQuery]CycleQuery query)
        {
            if (query.birthdate == null)
            {
                ModelState.AddModelError("birthdate", "Отсутствует значение параметра");
            }
            if (query.cycledate == null)
            {
                ModelState.AddModelError("cycledate", "Отсутствует значение параметра");
            }

            if (!string.IsNullOrEmpty(query.timezone) && query.city != null)
            {
                ModelState.AddModelError("timezone", "Необходимо указать только один параметр: город или timezone");
            }

            CheckDateParam(query.birthdate, "birthdate", out DateTime birthDate);
            CheckDateParam(query.cycledate, "cycledate", out DateTime cycleDate);

            if (!Enum.TryParse(query.cycletype, out HdStructures.Cycle cycleType))
            {
                ModelState.AddModelError("cycletype", "Недопустимый формат типа цикла");
            }

            CheckCityParam(query.city, null);

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            if (query.city != null || !string.IsNullOrEmpty(query.timezone))
            {
                DateConvertResult res;
                if (!string.IsNullOrEmpty(query.timezone))
                    res = ConvertDate(query.timezone, birthDate);
                else
                    res = await ConvertDate(query.city, birthDate);
                birthDate = res.dateConvertOut;
                if (res.errorType == -1)
                {
                    return BadRequest(new ResponseError
                    {
                        error = true,
                        message = "City not found"
                    });
                }
                if (res.errorType == -2)
                {
                    return BadRequest(new ResponseError
                    {
                        error = true,
                        message = "Date not converted"
                    });
                }
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
        public async Task<ActionResult> GetRaveTransitChartJson([FromQuery]TransitQuery query)
        {
            if (query.birthdate == null)
            {
                ModelState.AddModelError("birthdate", "Отсутствует значение параметра");
            }
            if (query.transitdate == null)
            {
                ModelState.AddModelError("transitdate", "Отсутствует значение параметра");
            }

            if (!string.IsNullOrEmpty(query.timezone) && query.city != null)
            {
                ModelState.AddModelError("timezone", "Необходимо указать только один параметр: город или timezone");
            }

            CheckDateParam(query.birthdate, "birthdate", out DateTime birthDate);

            CheckDateParam(query.birthdate, "transitDate", out DateTime transitDate);

            CheckCityParam(query.city, null);


            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }
            if (query.city != null || !string.IsNullOrEmpty(query.timezone))
            {
                DateConvertResult res;
                if (!string.IsNullOrEmpty(query.timezone))
                    res = ConvertDate(query.timezone, birthDate);
                else
                    res = await ConvertDate(query.city, birthDate);
                birthDate = res.dateConvertOut;
                if (res.errorType == -1)
                {
                    return BadRequest(new ResponseError
                    {
                        error = true,
                        message = "City not found"
                    });
                }
                if (res.errorType == -2)
                {
                    return BadRequest(new ResponseError
                    {
                        error = true,
                        message = "Date not converted"
                    });
                }
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
        public async Task<ActionResult> GetRaveConnectionChartJson([FromQuery]ConnectionQuery query)
        {
            List<DateTime> birthdates = new List<DateTime>();

            if(query.birthdates.Count != 2)
            {
                ModelState.AddModelError("birthdates", "Необходимо два параметра даты рождения");
            }

            for (int i = 0; i < query.birthdates.Count; i++)
            {
                Query birthdate = query.birthdates[i];
                if (birthdate != null)
                {
                    CheckDateParam(birthdate.birthdate, $"{i + 1}", out DateTime birthDate);
                    birthdates.Add(birthDate);
                }
                CheckCityParam(birthdate.city, i + 1);

                if (!string.IsNullOrEmpty(birthdate.timezone) && birthdate.city != null)
                {
                    ModelState.AddModelError("timezone", "Необходимо указать только один параметр: город или timezone");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }

            for (int i = 0; i < query.birthdates.Count; i++)
            {
                Query birthdate = query.birthdates[i];
                if (birthdate.city != null || !string.IsNullOrEmpty(birthdate.timezone))
                {
                    DateConvertResult res;
                    DateTime convbirthdate;

                    if (!string.IsNullOrEmpty(birthdate.timezone))
                        res = ConvertDate(birthdate.timezone, birthdates[i]);
                    else
                        res = await ConvertDate(birthdate.city, birthdates[i]);

                    convbirthdate = res.dateConvertOut;
                    if (res.errorType == -1)
                    {
                        return BadRequest(new ResponseError
                        {
                            error = true,
                            message = $"City{i + 1} not found"
                        });
                    }
                    if (res.errorType == -2)
                    {
                        return BadRequest(new ResponseError
                        {
                            error = true,
                            message = $"Date{i + 1} not converted"
                        });
                    }
                    birthdates[i] = convbirthdate;
                }
            }

            var rave = _raveRepo.GetConnectionChartInJson(birthdates[0], birthdates[1]);
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
            });
        }

        [HttpGet]
        [Route("rave-pentamodel")]
        public async Task<ActionResult> GetRavePentaModelJson([FromQuery]PentaQuery query)
        {
            List<DateTime> birthdates = new List<DateTime>();
            if (query.birthdates == null)
            {
                ModelState.AddModelError("birthdates", "Отсутствует значение параметра");
            }

            if(query.birthdates.Count < 3)
            {
                ModelState.AddModelError("birthdates", "Минимальное количество параметров равно 3");
            }

            if(query.birthdates.Count > 5)
            {
                ModelState.AddModelError("birthdates", "Максимальное количество параметров равно 5");
            }

            for(int i = 0; i < query.birthdates.Count; i++)
            {
                Query birthdate = query.birthdates[i];
                if (birthdate != null)
                {
                    CheckDateParam(birthdate.birthdate, $"{i + 1}", out DateTime birthDate);
                    birthdates.Add(birthDate);
                }
                CheckCityParam(birthdate.city, i + 1);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = GetModelStateError()
                });
            }

            for(int i = 0; i < query.birthdates.Count; i++) 
            {
                Query birthdate = query.birthdates[i];
                if (birthdate.city != null || !string.IsNullOrEmpty(birthdate.timezone))
                {
                    DateTime convbirthdate;
                    DateConvertResult res;

                    if (!string.IsNullOrEmpty(birthdate.timezone))
                        res = ConvertDate(birthdate.timezone, birthdates[i]);
                    else
                        res = await ConvertDate(birthdate.city, birthdates[i]);

                    convbirthdate = res.dateConvertOut;
                    if (res.errorType == -1)
                    {
                        return BadRequest(new ResponseError
                        {
                            error = true,
                            message = $"City{i + 1} not found"
                        });
                    }
                    if (res.errorType == -2)
                    {
                        return BadRequest(new ResponseError
                        {
                            error = true,
                            message = $"Date{i + 1} not converted"
                        });
                    }
                    birthdates[i] = convbirthdate;
                }
            }
            var rave = _raveRepo.GetPentaModelInJson(birthdates);
            return Ok(new ResponseResult<object>()
            {
                error = false,
                result = rave
            });
        }

        private void CheckDateParam(string dateParam, string dateParamName, out DateTime dateOut)
        {
            if (!DateTime.TryParse(dateParam, null, System.Globalization.DateTimeStyles.RoundtripKind, out dateOut))
            {
                ModelState.AddModelError(dateParamName, "Недопустимый формат даты");
            }
            else if (dateOut.Year < 1200)
            {
                ModelState.AddModelError(dateParamName, "Год должен быть больше 1200");

            }
        }

        private DateConvertResult ConvertDate(string timezone, DateTime inDateTime)
        {
            DateConvertResult result = new DateConvertResult();
            result.dateConvertOut = inDateTime;
            result.errorType = 0;
            if (string.IsNullOrEmpty(timezone))
            {
                return result;
            }
            if (!_timeZoneCorrector.ConvertToUtcFromCustomTimeZone(timezone, (DateTime)inDateTime, out result.dateConvertOut))
            {
                result.errorType = -2;
            }
            return result;
        }

        private async Task<DateConvertResult> ConvertDate(CityQuery city, DateTime inDateTime)
        {
            DateConvertResult result = new DateConvertResult();
            result.dateConvertOut = inDateTime;
            result.errorType = 0;
            if (city == null)
                return result;
            if (city.cityName != null)
            {
                var cityRes = await _timeZoneCorrector.GetCity(city.countryName, city.stateName, city.districtName, city.cityName);
                if (cityRes == null)
                {
                    result.errorType = -1;
                    return result;
                }
                if (_timeZoneCorrector.ConvertToUtcFromCustomTimeZone(cityRes.TimeZone,
                    (DateTime)inDateTime, out result.dateConvertOut))
                {
                    result.errorType = 0;
                    return result;
                }
                else
                {
                    result.errorType = -2;
                    return result;
                }
            }
            result.errorType = -1;
            return result;
        }

        private void CheckCityParam(CityQuery param, int? paramId)
        {
            if (param == null)
                return;
            if (param.cityName != null)
            {
                if (param.countryName == null)
                {
                    if(paramId.HasValue)
                        ModelState.AddModelError($"countryName{paramId.Value}", "Отсутствует значение параметра");
                    else
                        ModelState.AddModelError($"countryName", "Отсутствует значение параметра");
                }
            }
            if (param.countryName != null)
            {
                if (param.cityName == null)
                {
                    if(paramId.HasValue)
                        ModelState.AddModelError($"cityName{paramId.Value}", "Отсутствует значение параметра");
                    else
                        ModelState.AddModelError($"cityName", "Отсутствует значение параметра");
                }
            }
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