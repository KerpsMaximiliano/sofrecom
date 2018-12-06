using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Sofco.DAL;

namespace Sofco.WebApi.Infrastructures
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SofcoContext>
    {
        public SofcoContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<SofcoContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(
                connectionString, 
                 b => b.EnableRetryOnFailure()
                .MigrationsAssembly("Sofco.WebApi")
                .MigrationsHistoryTable(HistoryRepository.DefaultTableName, SofcoContext.AppSchemaName));

            return new SofcoContext(builder.Options);
        }
    }
}
