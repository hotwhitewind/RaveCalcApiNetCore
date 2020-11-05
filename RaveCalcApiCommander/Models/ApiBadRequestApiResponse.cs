using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.Models
{
    public class ApiResponse
    {
        public int StatusCode { get; }

        public string Message { get; }

        public ApiResponse(int statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);
        }

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    return "Resource not found";
                case 500:
                    return "An unhandled error occurred";
                case 401:
                    return "Unauthorized";
                default:
                    return null;
            }
        }
    }
}
