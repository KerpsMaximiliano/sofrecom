using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Sofco.Common.Logger
{
    public class Log4NetProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLogger> loggers =
            new ConcurrentDictionary<string, Log4NetLogger>();

        public void Dispose()
        {
            loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, new Log4NetLogger(categoryName));
        }
    }
}
