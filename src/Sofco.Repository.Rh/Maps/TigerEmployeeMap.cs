using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class TigerEmployeeMap : EntityMappingConfiguration<TigerEmployee>
    {
        public override void Map(EntityTypeBuilder<TigerEmployee> builder)
        {
            builder.ToTable("Recursos");

            builder.HasKey(s => s.Legaj);
        }
    }
}
