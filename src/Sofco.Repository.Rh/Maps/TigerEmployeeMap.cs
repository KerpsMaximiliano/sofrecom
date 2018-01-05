using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Extensions;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Repository.Rh.Maps
{
    public class TigerEmployeeMap : EntityMappingConfiguration<TigerEmployee>
    {
        public override void Map(EntityTypeBuilder<TigerEmployee> builder, RhSetting setting)
        {
            var tigerEmployeeTable = setting.TigerEmployeeTable;

            builder.ToTable(tigerEmployeeTable);

            builder.HasKey(s => s.Legaj);
        }
    }
}
