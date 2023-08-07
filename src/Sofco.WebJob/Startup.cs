using System;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Hangfire;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Framework.Logger.Extensions;
using Sofco.Repository.Rh.Settings;
using Sofco.Service.Crm.Settings;
using Sofco.Service.Settings;
using Sofco.Service.Settings.Jobs;
using Sofco.WebJob.Filters;
using Sofco.WebJob.Infrastructures;
using Sofco.WebJob.Services;

namespace Sofco.WebJob
{
    public class Startup
    {
        private const string WebJobPath = "/panel";

        private IContainer container;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddMvc();

            //services.AddHangfire(x => x
            //    .UseSqlServerStorage(Configuration.GetConnectionString("WebJobConnection"),
            //    new Hangfire.SqlServer.SqlServerStorageOptions
            //    {
            //        SchemaName = "HangFire_2"
            //    }
            //    ));

            services.AddHangfire(x => x
                .UseSqlServerStorage(Configuration.GetConnectionString("WebJobConnection")));

            services.Configure<EmailConfig>(Configuration.GetSection("Mail"));
            services.Configure<CrmConfig>(Configuration.GetSection("CRM"));
            services.Configure<RhSetting>(Configuration.GetSection("RhSetting"));
            services.Configure<JobSetting>(Configuration.GetSection("JobSetting"));
            services.Configure<AzureAdConfig>(Configuration.GetSection("AzureAd"));
            services.Configure<AppSetting>(Configuration.GetSection("App"));
            services.Configure<CrmSetting>(Configuration.GetSection("CRM"));

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new DefaultModule { Configuration = Configuration });
            containerBuilder.RegisterModule(new DatabaseModule { Configuration = Configuration });
            containerBuilder.RegisterModule(new AutoMapperModule());

            containerBuilder.Populate(services);

            container = containerBuilder.Build();

            JobActivator.Current = new AutofacJobActivator(container);

            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseHangfireDashboard(WebJobPath, new DashboardOptions
            {
                Authorization = new[] { new CustomAuthorizeFilter() }
            });

            app.UseHangfireServer();

            ConfigureLogger(loggerFactory);

            JobStartup.Init();
        }

        private void ConfigureLogger(ILoggerFactory loggerFactory)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            var log4NetConfig = Configuration.GetSection("log4NetConfig").Value;

            XmlConfigurator.Configure(logRepository, new FileInfo(log4NetConfig));

            loggerFactory.AddLog4Net(
                container.Resolve<IMailSender>(),
                container.Resolve<IMailBuilder>(),
                Configuration["Mail:SupportMailLogTitle"]);
        }
    }
}