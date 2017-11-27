using System;
using Microsoft.Extensions.Options;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Logger;
using Sofco.Core.Mail;

namespace Sofco.Framework.Logger
{
    public class LogMailer<T> : ILogMailer<T>
    {
        private readonly string mailLogSubject;

        private readonly ILoggerWrapper<T> logger;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public LogMailer(ILoggerWrapper<T> logger, IMailBuilder mailBuilder, IMailSender mailSender, IOptions<EmailConfig> emailConfigOption)
        {
            this.logger = logger;

            this.mailBuilder = mailBuilder;

            this.mailSender = mailSender;

            mailLogSubject = emailConfigOption.Value.SupportMailLogTitle;
        }

        public void LogError(Exception exception)
        {
            logger.LogError(exception);

            SendMail(exception);
        }

        private void SendMail(Exception exception)
        {
            var content = exception.Message + "<br><br>" + exception.StackTrace;

            var mail = mailBuilder.GetSupportEmail(mailLogSubject, content);

            mailSender.Send(mail);
        }
    }
}
