using System;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Sofco.Common.Logger
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog logger;

        public Log4NetLogger(string name)
        {
            logger = LogManager.GetLogger(Assembly.GetEntryAssembly(), name);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message;

            if (null != formatter)
            {
                message = formatter(state, exception);
            }
            else
            {
                message = exception.Message;
            }

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
    }
}