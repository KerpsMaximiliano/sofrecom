using System;
using System.Linq;
using System.Net.Http;
using Autofac;
using Microsoft.Extensions.Configuration;
using Sofco.Common.Helpers;
using Sofco.Common.Logger;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.Logger;
using Sofco.Framework.Mail;
using Sofco.Framework.StatusHandlers.Invoice;
using Sofco.Framework.StatusHandlers.Solfac;
using StackExchange.Redis;

namespace Sofco.WebApi.Infrastructures
{
    public class DefaultModule : Module
    {
        private const string ManagerAssemblyEndName = "Manager";
        private const string RepositoryAssemblyEndName = "Repository";
        private const string ServiceAssemblyEndName = "Service";
        private const string ClientAssemblyEndName = "Client";
        private const string DataAssemblyEndName = "Data";

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

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(DataAssemblyEndName))
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

            builder.RegisterType<SolfacStatusFactory>().As<ISolfacStatusFactory>();
            builder.RegisterType<InvoiceStatusFactory>().As<IInvoiceStatusFactory>();

            RegisterRedisDependencies(builder);

            RegisterLogger(builder);
        }

        private static void RegisterLogger(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(LoggerWrapper<>))
                .As(typeof(ILoggerWrapper<>));

            builder.RegisterGeneric(typeof(LogMailer<>))
                .As(typeof(ILogMailer<>));
        }

        private void RegisterRedisDependencies(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var redisConfig = Configuration["Redis:ConnectionString"];

                    var lazyConnection =
                        new Lazy<ConnectionMultiplexer>(
                            () => ConnectionMultiplexer.Connect(redisConfig));
                    return lazyConnection.Value;
                })
                .As<ConnectionMultiplexer>()
                .SingleInstance();

            builder.Register(c => c.Resolve<Lazy<ConnectionMultiplexer>>().Value.GetDatabase())
                .As<IDatabase>();
        }
    }
}
