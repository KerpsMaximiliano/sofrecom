using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Sofco.Repository.Rh.Extensions;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Repository.Rh
{
    public class TigerContext : DbContext
    {
        private readonly RhSetting setting;

        public TigerContext(DbContextOptions<TigerContext> options, RhSetting rhSetting)
            : base(options)
        {
            setting = rhSetting;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(setting.TigerSchema);

            builder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly, setting);
        }
    }
}
