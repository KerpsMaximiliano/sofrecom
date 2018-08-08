using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sofco.DAL;
using Sofco.Repository.Rh;
using Sofco.Repository.Rh.Settings;

namespace Sofco.WebApi.Infrastructures
{
    public class DatabaseModule : Module
    {
        private const string DefaultConnectionString = "DefaultConnection";

        private const string TigerConnectionString = "TigerConnection";

        public IConfigurationRoot Configuration { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterContext(builder);
        }

        private void RegisterContext(ContainerBuilder builder)
        {
            var dbBuilder = new DbContextOptionsBuilder<ReportContext>();

            dbBuilder.UseSqlServer(Configuration.GetConnectionString(DefaultConnectionString));

            builder.Register(r => new ReportContext(dbBuilder.Options));

            var tigerDbBuilder = new DbContextOptionsBuilder<TigerContext>();

            tigerDbBuilder.UseSqlServer(Configuration.GetConnectionString(TigerConnectionString));

            var rhSetting = new RhSetting();

            Configuration.GetSection("RhSetting").Bind(rhSetting);

            builder.Register(r => new TigerContext(tigerDbBuilder.Options, rhSetting));
        }
    }
}
