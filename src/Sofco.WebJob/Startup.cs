﻿using System;
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
            services.AddMvc();

            services.AddHangfire(x => x
                .UseSqlServerStorage(Configuration.GetConnectionString("WebJobConnection"))
            );

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<DefaultModule>();
            containerBuilder.RegisterModule(new DatabaseModule()
            {
                Configuration = Configuration
            });

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

            app.UseBasicAuthentication(WebJobAuthenticationOptions.Config(Configuration["JobSetting:username"], Configuration["JobSetting:password"]));

            app.UseHangfireDashboard(WebJobPath, new DashboardOptions()
            {
                Authorization = new[] { new CustomAuthorizeFilter() }
            });

            app.UseHangfireServer();

            JobService.Init();
        }
    }
}
