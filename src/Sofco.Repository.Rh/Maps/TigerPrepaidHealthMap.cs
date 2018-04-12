using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class TigerPrepaidHealthMap : EntityMappingConfiguration<TigerPrepaidHealth>
    {
        private const string TableName = "View_tb032";

        public override void Map(EntityTypeBuilder<TigerPrepaidHealth> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => new { s.Obsoc, s.Ospla });
        }
    }
}
