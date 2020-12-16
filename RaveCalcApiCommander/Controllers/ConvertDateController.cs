using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaveCalcApiCommander.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneCorrectorLibrary.Abstraction;

namespace RaveCalcApiCommander.Controllers
{
    [Route("apiv2")]
    [ApiController]
    public class ConvertDateController : ControllerBase
    {
        private readonly ITimeZoneCorrector _timeZoneCorrector;

        public ConvertDateController(ITimeZoneCorrector timeZoneCorrector)
        {
            _timeZoneCorrector = timeZoneCorrector;
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
            if (_timeZoneCorrector.ConvertToUtcFromCustomTimeZone(query.timeZoneName, dateFormat, out DateTime utcDate))
            {
                return Ok(new ResponseResult<DateTime>()
                {
                    error = false,
                    result = utcDate
                });
            }
            else
            {
                return BadRequest(new ResponseError
                {
                    error = true,
                    message = "Date not converted"
                });
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
