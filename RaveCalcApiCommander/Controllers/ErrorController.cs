using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaveCalcApiCommander.Models;

namespace RaveCalcApiCommander.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("error/{code}")]
        public IActionResult Error(int code)
        {
            var result = new ApiResponse(code);
            return new ObjectResult(new ResponseError()
            {
                error = true,
                message = result.Message
            });
        }
    }
}