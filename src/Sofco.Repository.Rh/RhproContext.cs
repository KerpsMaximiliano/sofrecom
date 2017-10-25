using Microsoft.EntityFrameworkCore;
using Sofco.Repository.Rh.Extensions;
using System.Reflection;

namespace Sofco.Repository.Rh
{
    public class RhproContext : DbContext
    {
        public const string AppSchemaName = "rhpro";

        public RhproContext(DbContextOptions<RhproContext> options) 
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
