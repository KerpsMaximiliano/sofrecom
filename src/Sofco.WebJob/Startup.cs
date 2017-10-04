using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Hangfire;
using Sofco.WebJob.Services;
using Sofco.WebJob.Filters;
using Sofco.WebJob.Security.Events;

namespace Sofco.WebJob
{
    public class Startup
    {
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddHangfire(x => x
                .UseSqlServerStorage(Configuration.GetConnectionString("WebJobConnection"))
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseBasicAuthentication(new BasicAuthenticationOptions
            {
                Events = new BasicAuthenticationEvents
                {
                    OnValidateCredentials = context =>
                    {
                        if (context.Username == context.Password)
                        {
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, context.Username)
                            };
                            context.Ticket = new AuthenticationTicket(
                                new ClaimsPrincipal(
                                    new ClaimsIdentity(claims, context.Options.AuthenticationScheme)),
                                new AuthenticationProperties(),
                                context.Options.AuthenticationScheme);
                        }

                        return Task.FromResult<object>(null);
                    }
                }
            });

            app.UseHangfireDashboard("/jobs", new DashboardOptions()
            {
                Authorization = new[] { new CustomAuthorizeFilter() }
            });

            app.UseHangfireServer();

            JobService.Init();
        }
    }
}
