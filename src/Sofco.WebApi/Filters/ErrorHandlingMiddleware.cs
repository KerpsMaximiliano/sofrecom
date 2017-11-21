using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Mail;

namespace Sofco.WebApi.Filters
{
    public class ErrorHandlingMiddleware
    {
        private readonly string mailLogSubject;

        private readonly RequestDelegate next;

        private readonly ILoggerWrapper<ErrorHandlingMiddleware> log;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerWrapper<ErrorHandlingMiddleware> log, IMailBuilder mailBuilder, IMailSender mailSender, IOptions<EmailConfig> emailConfigOption)
        {
            this.next = next;

            this.log = log;

            this.mailBuilder = mailBuilder;

            this.mailSender = mailSender;

            mailLogSubject = emailConfigOption.Value.SupportMailLogTitle;
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
            log.LogError(exception);

            SendMail(exception);

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

        private void SendMail(Exception exception)
        {
            var content = exception.Message + "<br><br>" + exception.StackTrace;

            var mail = mailBuilder.GetSupportEmail(mailLogSubject, content);

            mailSender.Send(mail);
        }
    }
}
