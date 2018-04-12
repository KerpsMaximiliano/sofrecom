using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class TigerHealthInsuranceMap : EntityMappingConfiguration<TigerHealthInsurance>
    {
        private const string TableName = "View_tb031";

        public override void Map(EntityTypeBuilder<TigerHealthInsurance> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => s.Obsoc);
        }
    }
}
