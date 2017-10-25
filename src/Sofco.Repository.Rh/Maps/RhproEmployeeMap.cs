using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class RhproEmployeeMap : EntityMappingConfiguration<RhproEmployee>
    {
        public override void Map(EntityTypeBuilder<RhproEmployee> builder)
        {
            builder.ToTable("empleado");

            builder.HasKey(s => s.Empleg);
        }
    }
}
