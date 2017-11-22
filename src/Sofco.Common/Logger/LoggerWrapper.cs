using System;
using log4net;
using Sofco.Common.Logger.Interfaces;

namespace Sofco.Common.Logger
{
    public class LoggerWrapper<T> : ILoggerWrapper<T>
    {
        private readonly ILog log;

        public LoggerWrapper()
        {
            log = LogManager.GetLogger(typeof(T));
        }

        public void LogError(string message)
        {
            log.Error(message);
        }

        public void LogError(Exception exception)
        {
            log.Error(exception.Message, exception);
        }
    }
}
