using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaveCalcApiCommander.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RaveCalcApiCommander.GlobalErrorHandling
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger _logger;
        
        public CustomExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            _logger = loggerFactory
                  .CreateLogger<CustomExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (HttpStatusCodeException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception exceptionObj)
            {
                await HandleExceptionAsync(context, exceptionObj);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, HttpStatusCodeException exception)
        {
            string result = null;
            context.Response.ContentType = "application/json";
            if (exception is HttpStatusCodeException)
            {
                _logger.LogError(exception.Message);
                result = new ResponseError
                {
                    error = true,
                    message = $"{(int)exception.StatusCode} - {exception.Message}"
                }.ToString();
                context.Response.StatusCode = (int)exception.StatusCode;
            }
            else
            {
                _logger.LogError("Runtime Error");

                result = new ResponseError
                {
                    error = true,
                    message = $"{(int)HttpStatusCode.InternalServerError} - Runtime Error"
                }.ToString();

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            return context.Response.WriteAsync(result);
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception.Message);
            string result = new ResponseError
            {
                error = true,
                message = $"{(int)HttpStatusCode.InternalServerError} - {exception.Message}"
            }.ToString();

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }

    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ContentType { get; set; } = @"text/plain";

        public HttpStatusCodeException(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.StatusCode = statusCode;
        }

        public HttpStatusCodeException(HttpStatusCode statusCode, Exception inner) : this(statusCode, inner.ToString()) { }

        public HttpStatusCodeException(HttpStatusCode statusCode, JObject errorObject) : this(statusCode, errorObject.ToString())
        {
            this.ContentType = @"application/json";
        }

    }

    internal class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
