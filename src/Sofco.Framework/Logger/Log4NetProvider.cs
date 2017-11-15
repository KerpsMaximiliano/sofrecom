using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Sofco.Core.Mail;

namespace Sofco.Framework.Logger
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogger> loggers =
            new ConcurrentDictionary<string, Log4NetLogger>();

        private readonly IMailSender mailSender;

        private readonly IMailBuilder mailBuilder;

        private readonly string mailLogSubject;

        public Log4NetProvider(IMailSender mailSender, IMailBuilder mailBuilder, string mailLogSubject)
        {
            this.mailSender = mailSender;

            this.mailBuilder = mailBuilder;

            this.mailLogSubject = mailLogSubject;
        }

        public void Dispose()
        {
            loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, 
                new Log4NetLogger(categoryName, 
                    mailSender, 
                    mailBuilder, 
                    mailLogSubject));
        }
    }
}
