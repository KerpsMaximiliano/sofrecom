using System.Linq;
using System.Net.Http;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sofco.Common.Helpers;
using Sofco.Common.Logger;
using Sofco.Common.Logger.Interfaces;
using Sofco.WebJob.Jobs.Interfaces;
using Sofco.Core.Mail;
using Sofco.Framework.Mail;
using Sofco.Service.Http;
using Sofco.Service.Http.Interfaces;

namespace Sofco.WebJob.Infrastructures
{
    public class DefaultModule : Module
    {
        private const string ServiceAssemblyEndName = "Service";

        public IConfigurationRoot Configuration { set; get; }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(nameof(Sofco)).ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                .AssignableTo<IJob>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(ServiceAssemblyEndName))
                .AsImplementedInterfaces();
            
            builder.RegisterType<MailSender>()
                .As<IMailSender>()
                .SingleInstance();

            builder.RegisterGeneric(typeof(BaseHttpClient<>))
                .As(typeof(IBaseHttpClient<>))
                .WithParameter(new TypedParameter(typeof(HttpClient), new HttpClient()))
                .SingleInstance();
            RegisterLogger(builder);
        }

        private static void RegisterLogger(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(LoggerWrapper<>))
                .As(typeof(ILoggerWrapper<>));
        }

    }
}
