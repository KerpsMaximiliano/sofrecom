using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class RhpropLicenseTypeMap : EntityMappingConfiguration<RhproLicenseType>
    {
        const string TableName = "tipdia";

        public override void Map(EntityTypeBuilder<RhproLicenseType> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => s.Tdnro);
        }
    }
}
