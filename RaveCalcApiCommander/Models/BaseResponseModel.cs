using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Models
{
    public class ResponseResult<T>
    {
        public bool error { get; set; }
        public T result { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class ResponseError
    {
        public bool error { get; set; }
        public string message { get; set; }
    }

    public class UserToken
    {
        public string UserName { get; set; }
        public string Token { get; set; }
    }

    public class DateConvertResult
    {
        public DateTime dateConvertOut;
        public int errorType;
    }
}
