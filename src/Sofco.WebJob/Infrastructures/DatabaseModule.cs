using System.Linq;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sofco.Common.Helpers;
using Sofco.DAL;

namespace Sofco.WebJob.Infrastructures
{
    public class DatabaseModule : Module
    {
        private const string RepositoryAssemblyEndName = "Repository";

        public IConfigurationRoot Configuration { set; get; }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(nameof(Sofco)).ToArray();

            var dbBuilder = new DbContextOptionsBuilder<SofcoContext>();

            dbBuilder.UseSqlServer(Configuration.GetConnectionString("WebJobConnection"));

            builder.Register(r => new SofcoContext(dbBuilder.Options));

            builder.RegisterAssemblyTypes(assemblies)
                .Where(s => s.Name.EndsWith(RepositoryAssemblyEndName))
                .AsImplementedInterfaces();
        }
    }
}