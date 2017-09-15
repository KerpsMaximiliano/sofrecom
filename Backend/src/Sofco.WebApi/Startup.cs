using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Core.FileManager;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.DAL;
using Sofco.DAL.Repositories.Admin;
using Sofco.DAL.Repositories.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Framework.FileManager.Billing;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;
using Sofco.Service.Implementations.Admin;
using Sofco.Service.Implementations.Billing;
using Sofco.Service.Implementations.Common;
using Sofco.WebApi.Config;

namespace Sofco.WebApi
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<SofcoContext>(options =>
                   options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Sofco.WebApi")));

            services.AddMvc().AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling =
                        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.AddOptions();
            services.AddCors();

            services.Configure<IISOptions>(options => {
                options.AutomaticAuthentication = true;
            });

            //Config
            services.Configure<EmailConfig>(Configuration.GetSection("Mail"));
            services.Configure<CrmConfig>(Configuration.GetSection("CRM"));
            services.Configure<AzureAdConfig>(Configuration.GetSection("AzureAd"));

            services.AddAuthentication(sharedOptions => sharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            // Services
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFunctionalityService, FunctionalityService>();
            services.AddTransient<IMenuService, MenuService>();
            services.AddTransient<IModuleService, ModuleService>();
            services.AddTransient<IUtilsService, UtilsService>();
            services.AddTransient<ISolfacService, SolfacService>();
            services.AddTransient<IInvoiceService, InvoiceService>();

            // Repositories
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IBaseRepository<Customer>, BaseRepository<Customer>>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IBaseRepository<Group>, BaseRepository<Group>>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IBaseRepository<Role>, BaseRepository<Role>>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserGroupRepository, UserGroupRepository>();
            services.AddTransient<IFunctionalityRepository, FunctionalityRepository>();
            services.AddTransient<IRoleFunctionalityRepository, RoleFunctionalityRepository>();
            services.AddTransient<IModuleRepository, ModuleRepository>();
            services.AddTransient<IRoleFunctionalityRepository, RoleFunctionalityRepository>();
            services.AddTransient<IMenuRepository, MenuRepository>();
            services.AddTransient<IUtilsRepository, UtilsRepository>();
            services.AddTransient<ISolfacRepository, SolfacRepository>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();

            // File Manager
            services.AddTransient<IInvoiceFileManager, InvoiceFileManager>();
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

            app.UseCors(builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseMvc();
        }
    }
}
