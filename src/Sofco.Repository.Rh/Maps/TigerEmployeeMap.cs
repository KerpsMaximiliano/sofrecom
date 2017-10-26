using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class TigerEmployeeMap : EntityMappingConfiguration<TigerEmployee>
    {
        const string TableName = "Recursos";

        public override void Map(EntityTypeBuilder<TigerEmployee> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => s.Legaj);
        }
    }
}
