using System.Linq;
using System.Net.Http;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sofco.Common.Helpers;
using Sofco.Common.Logger;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.DAL;
using Sofco.Framework.Logger;
using Sofco.Framework.Mail;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Infrastructures
{
    public class DefaultModule : Module
    {
        private const string ServiceAssemblyEndName = "Service";
        private const string ClientAssemblyEndName = "Client";

        public IConfigurationRoot Configuration { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(nameof(Sofco)).ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IJob>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(ServiceAssemblyEndName))
                .AsImplementedInterfaces();

            builder.RegisterType<MailBuilder>()
                .As<IMailBuilder>();

            builder.RegisterType<MailSender>()
                .As<IMailSender>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(ClientAssemblyEndName))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<HttpClient>()
                .SingleInstance();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            RegisterLogger(builder);
        }

        private static void RegisterLogger(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(LoggerWrapper<>))
                .As(typeof(ILoggerWrapper<>));

            builder.RegisterGeneric(typeof(LogMailer<>))
                .As(typeof(ILogMailer<>));
        }
    }
}
