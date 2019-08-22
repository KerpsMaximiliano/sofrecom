using System;
using System.Linq;
using System.Net.Http;
using Autofac;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Sofco.Common.Helpers;
using Sofco.Common.Logger;
using Sofco.Common.Logger.Interfaces;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Services.Common;
using Sofco.DAL;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Recruitment;
using Sofco.Framework.Logger;
using Sofco.Framework.Mail;
using Sofco.Service.Crm.HttpClients;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Translators;
using Sofco.Service.Crm.Translators.Interfaces;
using Sofco.Service.Implementations.Common;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace Sofco.WebApi.Infrastructures
{
    public class DefaultModule : Module
    {
        private const string ManagerAssemblyEndName = "Manager";
        private const string RepositoryAssemblyEndName = "Repository";
        private const string ServiceAssemblyEndName = "Service";
        private const string ClientAssemblyEndName = "Client";
        private const string DataAssemblyEndName = "Data";
        private const string ValidationAssemblyEndName = "Validation";
        private const string FactoryAssemblyEndName = "Factory";

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

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(ValidationAssemblyEndName))
                .AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(CrmTranslator<,>))
                .As(typeof(ICrmTranslator<,>));

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(FactoryAssemblyEndName))
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

            builder.RegisterType<CrmApiHttpClient>()
                .As<ICrmApiHttpClient>()
                .SingleInstance();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>();

            builder.RegisterType<OptionRepository<Seniority>>().As<IOptionRepository<Seniority>>();
            builder.RegisterType<OptionService<Seniority>>().As<IOptionService<Seniority>>();

            builder.RegisterType<OptionRepository<Skill>>().As<IOptionRepository<Skill>>();
            builder.RegisterType<OptionService<Skill>>().As<IOptionService<Skill>>();

            builder.RegisterType<OptionRepository<Profile>>().As<IOptionRepository<Profile>>();
            builder.RegisterType<OptionService<Profile>>().As<IOptionService<Profile>>();

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
