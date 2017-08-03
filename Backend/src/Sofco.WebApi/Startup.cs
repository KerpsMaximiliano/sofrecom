using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sofco.Core.DAL;
using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Core.Services;
using Sofco.DAL;
using Sofco.DAL.Repositories;
using Sofco.Model.Models;
using Sofco.Service.Implementations;
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

            services.Configure<ActiveDirectoryConfig>(Configuration.GetSection("ActiveDirectory"));

            // Services
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFunctionalityService, FunctionalityService>();
            services.AddTransient<IMenuService, MenuService>();

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
            services.AddTransient<IRoleMenuRepository, RoleMenuRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseMvc();
        }
    }
}
