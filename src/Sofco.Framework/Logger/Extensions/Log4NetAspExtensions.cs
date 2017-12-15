using Microsoft.Extensions.Logging;
using Sofco.Core.Mail;

namespace Sofco.Framework.Logger.Extensions
{
    public static class Log4NetAspExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, 
            IMailSender mailSender, 
            IMailBuilder mailBuilder,
            string mailLogSubject)
        {
            factory.AddProvider(new Log4NetProvider(mailSender, mailBuilder, mailLogSubject));

            return factory;
        }
    }
}
