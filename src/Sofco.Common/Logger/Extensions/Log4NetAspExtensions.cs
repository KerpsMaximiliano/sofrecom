using Microsoft.Extensions.Logging;

namespace Sofco.Common.Logger.Extensions
{
    public static class Log4NetAspExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
        {
            factory.AddProvider(new Log4NetProvider());

            return factory;
        }
    }
}
