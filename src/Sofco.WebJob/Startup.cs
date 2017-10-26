using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Sofco.WebJob.Services;
using Sofco.WebJob.Filters;
using Sofco.WebJob.Security;
using Sofco.WebJob.Infrastructures;
using Sofco.Core.Config;
using Sofco.Service.Settings.Jobs;
using Sofco.Repository.Rh.Settings;

namespace Sofco.WebJob
{
    public class Startup
    {
        private const string WebJobPath = "/panel";

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

            services.AddHangfire(x => x
                .UseSqlServerStorage(Configuration.GetConnectionString("WebJobConnection"))
            );

            services.Configure<EmailConfig>(Configuration.GetSection("Mail"));
            services.Configure<CrmConfig>(Configuration.GetSection("CRM"));
            services.Configure<SolfacJobSetting>(Configuration.GetSection("SolfacJob"));
            services.Configure<RhSetting>(Configuration.GetSection("RhSetting"));

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new DefaultModule(){ Configuration = Configuration });
            containerBuilder.RegisterModule(new DatabaseModule() { Configuration = Configuration });
            containerBuilder.RegisterModule(new AutoMapperModule());

            containerBuilder.Populate(services);

            var container = containerBuilder.Build();

            JobActivator.Current = new AutofacJobActivator(container);

            return new AutofacServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseBasicAuthentication(WebJobAuthenticationOptions.Config(Configuration["JobSetting:PanelUsername"], Configuration["JobSetting:PanelPassword"]));

            app.UseHangfireDashboard(WebJobPath, new DashboardOptions()
            {
                Authorization = new[] { new CustomAuthorizeFilter() }
            });

            app.UseHangfireServer();

            JobService.Init(Configuration["JobSetting:LocalTimeZoneName"]);
        }
    }
}
