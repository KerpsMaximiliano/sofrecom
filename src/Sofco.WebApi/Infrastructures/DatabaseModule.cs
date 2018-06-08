using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sofco.DAL;

namespace Sofco.WebApi.Infrastructures
{
    public class DatabaseModule : Module
    {
        private const string DefaultConnectionString = "DefaultConnection";

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
        }
    }
}
