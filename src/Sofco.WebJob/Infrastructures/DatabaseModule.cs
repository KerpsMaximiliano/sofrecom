using System.Linq;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sofco.Common.Helpers;
using Sofco.DAL;
using Sofco.Repository.Rh;
using Sofco.Repository.Rh.Settings;

namespace Sofco.WebJob.Infrastructures
{
    public class DatabaseModule : Module
    {
        const string WebJobConnectionString = "WebJobConnection";

        const string TigerConnectionString = "TigerConnection";

        const string RhproConnectionString = "RhproConnection";

        private const string RepositoryAssemblyEndName = "Repository";

        public IConfigurationRoot Configuration { set; get; }

        protected override void Load(ContainerBuilder builder)
        {
            var assemblies = AssemblyHelper.GetReferencingAssemblies(nameof(Sofco)).ToArray();

            RegisterContext(builder);

            RegisterRepositories(builder, assemblies);
        }

        private void RegisterContext(ContainerBuilder builder)
        {
            var dbBuilder = new DbContextOptionsBuilder<SofcoContext>();

            dbBuilder.UseSqlServer(Configuration.GetConnectionString(WebJobConnectionString));

            builder.Register(r => new SofcoContext(dbBuilder.Options));

            var tigerDbBuilder = new DbContextOptionsBuilder<TigerContext>();

            tigerDbBuilder.UseSqlServer(Configuration.GetConnectionString(TigerConnectionString));

            var rhSetting = new RhSetting();

            Configuration.GetSection("RhSetting").Bind(rhSetting);

            builder.Register(r => new TigerContext(tigerDbBuilder.Options, rhSetting));

            var rhProDbBuilder = new DbContextOptionsBuilder<RhproContext>();

            rhProDbBuilder.UseSqlServer(Configuration.GetConnectionString(RhproConnectionString));

            builder.Register(r => new RhproContext(rhProDbBuilder.Options, rhSetting));
        }

        private void RegisterRepositories(ContainerBuilder builder, System.Reflection.Assembly[] assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                    .Where(s => s.Name.EndsWith(RepositoryAssemblyEndName))
                    .AsImplementedInterfaces();
        }
    }
}