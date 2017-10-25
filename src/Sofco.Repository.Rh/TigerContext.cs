using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh
{
    public class TigerContext : DbContext
    {
        public const string AppSchemaName = "tiger";

        public TigerContext(DbContextOptions<TigerContext> options) 
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(AppSchemaName);

            builder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly);
        }
    }
}
