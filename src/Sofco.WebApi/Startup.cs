﻿using System;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.DAL;
using Sofco.Service.Settings;
using Sofco.WebApi.Filters;
using Sofco.WebApi.Infrastructures;

namespace Sofco.WebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SofcoContext>(options =>
                   options
                   .UseSqlServer(
                       Configuration.GetConnectionString("DefaultConnection"),
                       b => b
                       .MigrationsAssembly("Sofco.WebApi")
                       .MigrationsHistoryTable(HistoryRepository.DefaultTableName, SofcoContext.AppSchemaName)));

            services.AddMvc().AddJsonOptions(options =>
            {
                options
                .SerializerSettings
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new VersionHeaderFilter());
            });

            services.AddOptions();
            services.AddCors();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = true;
            });

            // Config
            services.Configure<EmailConfig>(Configuration.GetSection("Mail"));
            services.Configure<CrmConfig>(Configuration.GetSection("CRM"));
            services.Configure<AzureAdConfig>(Configuration.GetSection("AzureAd"));
            services.Configure<FileConfig>(Configuration.GetSection("File"));
            services.Configure<AppSetting>(Configuration.GetSection("App"));

            services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new DefaultModule { Configuration = Configuration });
            containerBuilder.RegisterModule(new AutoMapperModule());
            containerBuilder.RegisterModule(new DatabaseModule { Configuration = Configuration });

            containerBuilder.Populate(services);

            var container = containerBuilder.Build();

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                Authority = string.Format(Configuration["AzureAd:AadInstance"], Configuration["AzureAd:Tenant"]),
                Audience = Configuration["AzureAd:Audience"]
            });

            app.UseCors(builder => builder.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowCredentials()
                .WithExposedHeaders(VersionHeaderFilter.HeaderAppVersionName));
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            ConfigureLogger();
        }

        private static void ConfigureLogger()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
    }
}
