using System;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Logging;
using Sofco.Core.Mail;

namespace Sofco.Framework.Logger
{
    public class Log4NetLogger : ILogger
    {
        private readonly string mailLogSubject;

        private readonly ILog logger;

        private readonly IMailSender mailSender;

        private readonly IMailBuilder mailBuilder;

        public Log4NetLogger(string name, 
            IMailSender mailSender, 
            IMailBuilder mailBuilder,
            string mailLogSubject)
        {
            logger = LogManager.GetLogger(Assembly.GetEntryAssembly(), name);

            this.mailSender = mailSender;

            this.mailBuilder = mailBuilder;

            this.mailLogSubject = mailLogSubject;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = null != formatter ? formatter(state, exception) : exception.Message;

            SendMail(message, exception);

            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    logger.Debug(message, exception);
                    break;
                case LogLevel.Information:
                    logger.Info(message, exception);
                    break;
                case LogLevel.Warning:
                    logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    logger.Error(message, exception);
                    break;
                case LogLevel.Critical:
                    logger.Fatal(message, exception);
                    break;
                default:
                    logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                    logger.Info(message, exception);
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return logger.IsDebugEnabled;
                case LogLevel.Information:
                    return logger.IsInfoEnabled;
                case LogLevel.Warning:
                    return logger.IsWarnEnabled;
                case LogLevel.Error:
                    return logger.IsErrorEnabled;
                case LogLevel.Critical:
                    return logger.IsFatalEnabled;
                default:
                    throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel));
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private void SendMail(string message, Exception exception)
        {
            var content = message + "<br><br>" + exception.StackTrace;

            var mail = mailBuilder.GetSupportEmail(mailLogSubject, content);

            mailSender.Send(mail);
        }
    }
}