using System;
using Microsoft.Extensions.Options;
using Sofco.Common.Logger.Interfaces;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Logger;
using Sofco.Core.Mail;

namespace Sofco.Framework.Logger
{
    public class LogMailer<T> : ILogMailer<T>
    {
        private const string MessageDelimiter = " - ";

        private readonly string mailLogSubject;

        private readonly ILoggerWrapper<T> logger;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly ISessionManager sessionManager;

        public LogMailer(ILoggerWrapper<T> logger, IMailBuilder mailBuilder, IMailSender mailSender, IOptions<EmailConfig> emailConfigOption, ISessionManager sessionManager)
        {
            this.logger = logger;

            this.mailBuilder = mailBuilder;

            this.mailSender = mailSender;

            this.sessionManager = sessionManager;

            mailLogSubject = emailConfigOption.Value.SupportMailLogTitle;
        }

        public void LogError(string message, Exception exception)
        {
            logger.LogError(message, exception);

            SendMail(message, exception);
        }

        public void LogError(Exception exception)
        {
            logger.LogError(exception);

            SendMail(exception);
        }

        private void SendMail(Exception exception)
        {
            SendMail(string.Empty, exception);
        }

        private void SendMail(string message, Exception exception)
        {
            var msg = string.IsNullOrEmpty(message)
                ? exception.Message
                : message + MessageDelimiter + exception.Message;

            var content = GetUserName() + msg + "<br><br>" + exception.StackTrace;

            var mail = mailBuilder.GetSupportEmail(mailLogSubject, content);

            mailSender.Send(mail);
        }

        private string GetUserName()
        {
            var result = string.Empty;

            try
            {
                result = "Username: <b>" + sessionManager.GetUserName() + "</b><br><br>";
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }
    }
}
