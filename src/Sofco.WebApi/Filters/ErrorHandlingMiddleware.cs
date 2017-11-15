using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Sofco.Common.Logger.Interfaces;

namespace Sofco.WebApi.Filters
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILoggerWrapper<ErrorHandlingMiddleware> log;


        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerWrapper<ErrorHandlingMiddleware> log)
        {
            this.next = next;
            this.log = log;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            log.LogError(exception.Message, exception);

            var code = HttpStatusCode.InternalServerError;

            if (exception is KeyNotFoundException)
                code = HttpStatusCode.NotFound;

            var result = JsonConvert.SerializeObject(new
                {
                    success = false,
                    error = new { exception.Message, exception.StackTrace }
                });


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
