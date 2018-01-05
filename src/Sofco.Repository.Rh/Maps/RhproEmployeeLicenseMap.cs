using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Extensions;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Repository.Rh.Maps
{
    public class RhproEmployeeLicenseMap : EntityMappingConfiguration<RhproEmployeeLicense>
    {
        const string TableName = "emp_lic";

        public override void Map(EntityTypeBuilder<RhproEmployeeLicense> builder, RhSetting setting)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => s.Empleado);
        }
    }
}
