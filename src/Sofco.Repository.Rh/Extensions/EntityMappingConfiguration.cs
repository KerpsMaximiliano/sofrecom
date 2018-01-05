using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Repository.Rh.Extensions
{
    public abstract class EntityMappingConfiguration<T> : IEntityMappingConfiguration<T> where T : class
    {
        public abstract void Map(EntityTypeBuilder<T> builder, RhSetting setting);

        public void Map(ModelBuilder builder, RhSetting setting)
        {
            Map(builder.Entity<T>(), setting);
        }
    }
}
