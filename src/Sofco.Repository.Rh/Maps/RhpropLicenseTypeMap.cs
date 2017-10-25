using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class RhpropLicenseTypeMap : EntityMappingConfiguration<RhproLicenseType>
    {
        public override void Map(EntityTypeBuilder<RhproLicenseType> builder)
        {
            builder.ToTable("tipdia");

            builder.HasKey(s => s.Tdnro);
        }
    }
}
