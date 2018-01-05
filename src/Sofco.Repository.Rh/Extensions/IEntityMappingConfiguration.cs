using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Repository.Rh.Extensions
{
    public interface IEntityMappingConfiguration
    {
        void Map(ModelBuilder builer, RhSetting setting);
    }

    public interface IEntityMappingConfiguration<T> : IEntityMappingConfiguration where T : class
    {
        void Map(EntityTypeBuilder<T> builder, RhSetting setting);
    }
}
