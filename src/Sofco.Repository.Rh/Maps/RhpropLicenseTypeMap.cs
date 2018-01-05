using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Extensions;
using Sofco.Repository.Rh.Settings;

namespace Sofco.Repository.Rh.Maps
{
    public class RhpropLicenseTypeMap : EntityMappingConfiguration<RhproLicenseType>
    {
        const string TableName = "tipdia";

        public override void Map(EntityTypeBuilder<RhproLicenseType> builder, RhSetting setting)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => s.Tdnro);
        }
    }
}
