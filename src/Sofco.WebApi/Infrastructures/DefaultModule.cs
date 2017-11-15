using System.Linq;
using System.Net.Http;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sofco.Common.Helpers;
using Sofco.Common.Logger;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.Mail;
using Sofco.Framework.StatusHandlers.Invoice;
using Sofco.Framework.StatusHandlers.Solfac;
using Sofco.Service.Http;
using Sofco.Service.Http.Interfaces;

namespace Sofco.WebApi.Infrastructures
{
    public class DefaultModule : Module
    {
        private const string ManagerAssemblyEndName = "Manager";
        private const string RepositoryAssemblyEndName = "Repository";
        private const string ServiceAssemblyEndName = "Service";

        public IConfigurationRoot Configuration { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(nameof(Sofco)).ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(ServiceAssemblyEndName))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(ManagerAssemblyEndName))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(RepositoryAssemblyEndName))
                .AsImplementedInterfaces();

            builder.RegisterType<MailSender>()
                .As<IMailSender>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(BaseHttpClient<>))
                .As(typeof(IBaseHttpClient<>))
                .WithParameter(new TypedParameter(typeof(HttpClient), new HttpClient()))
                .SingleInstance();

            builder.RegisterType<SolfacStatusFactory>().As<ISolfacStatusFactory>();
            builder.RegisterType<InvoiceStatusFactory>().As<IInvoiceStatusFactory>();

            RegisterLogger(builder);
        }

        private static void RegisterLogger(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(LoggerWrapper<>))
                .As(typeof(ILoggerWrapper<>));
        }
    }
}
