using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Extensions;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Repository.Rh.Maps
{
    public class RhproEmployeeMap : EntityMappingConfiguration<RhproEmployee>
    {
        const string TableName = "empleado";

        public override void Map(EntityTypeBuilder<RhproEmployee> builder, RhSetting setting)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => s.Empleg);
        }
    }
}
