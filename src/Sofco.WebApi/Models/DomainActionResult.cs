using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofco.Common.Domains;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Sofco.WebApi.Models
{
    public class DomainActionResult : IActionResult
    {
        internal const string ContentProperty = "ResultData";

        internal readonly HttpContext Context;

        public Result Result { get; set; }

        protected DomainActionResult(HttpContext context)
        {
            Context = context;
        }

        public DomainActionResult(HttpContext context, Result result)
            : this(context)
        {
            Result = result;

            SetHttpStatusCode();
        }

        private void SetHttpStatusCode()
        {
            Context.Response.StatusCode = Result.HasErrors
                ? (int)HttpStatusCode.BadRequest // Http422UnprocessableEntity
                : (int)HttpStatusCode.OK;
        }

        internal Func<HttpRequestMessage, HttpStatusCode, object, HttpResponseMessage> CreateResponse { get; set; }

        private object GetContentsFromResult()
        {
            var t = Result.GetType();

            return
                t.GetProperty(ContentProperty) != null
                ? t.GetProperty(ContentProperty).GetValue(Result)
                : null;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var response = new ResponseModel
            {
                Data = GetContentsFromResult(),
                Errors = Result.Errors.ToList(),
                Status = Result.HasErrors
                   ? StatusType.Error.ToString().ToUpper()
                   : StatusType.Success.ToString().ToUpper()
            };

            var contextResponse = Context.Response;

            contextResponse.ContentType = "application/json; charset=utf-8";

            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return Context.Response.WriteAsync(JsonConvert.SerializeObject(response, jsonSettings), Encoding.UTF8);
        }
    }
}