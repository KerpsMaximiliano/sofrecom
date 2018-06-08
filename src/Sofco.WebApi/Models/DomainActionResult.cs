using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sofco.Common.Domains;
using Sofco.Core.Models;

namespace Sofco.WebApi.Models
{
    public class DomainActionResult : IActionResult
    {
        private const string ContentProperty = "ResultData";

        private readonly HttpContext context;

        public DomainActionResult(HttpContext context, Result result)
            : this(context)
        {
            Result = result;

            SetHttpStatusCode();
        }

        protected DomainActionResult(HttpContext context)
        {
            this.context = context;
        }

        public Result Result { get; set; }

        public Func<HttpRequestMessage, HttpStatusCode, object, HttpResponseMessage> CreateResponse { get; set; }

        public Task ExecuteResultAsync(ActionContext actionContext)
        {
            var response = new ResponseModel
            {
                Data = GetContentsFromResult(),
                Errors = Result.Errors.ToList(),
                Status = Result.HasErrors
                    ? StatusType.Error.ToString().ToUpper()
                    : StatusType.Success.ToString().ToUpper()
            };

            var contextResponse = context.Response;

            contextResponse.ContentType = "application/json; charset=utf-8";

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response, jsonSettings), Encoding.UTF8);
        }

        private void SetHttpStatusCode()
        {
            context.Response.StatusCode = Result.HasErrors
                ? (int)HttpStatusCode.BadRequest // Http422UnprocessableEntity
                : (int)HttpStatusCode.OK;
        }

        private object GetContentsFromResult()
        {
            var t = Result.GetType();

            return
                t.GetProperty(ContentProperty) != null
                ? t.GetProperty(ContentProperty).GetValue(Result)
                : null;
        }
    }
}