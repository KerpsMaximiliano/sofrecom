using Microsoft.EntityFrameworkCore;
using Sofco.Model.Utils;

namespace Sofco.DAL.Mappings.Utils
{
    public static class UtilsMapping
    {
        public static void MapUtils(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<DocumentType>().HasKey(_ => _.Id);
            builder.Entity<DocumentType>().Property(_ => _.Text).HasMaxLength(50);

            // Primary Key
            builder.Entity<Province>().HasKey(_ => _.Id);
            builder.Entity<Province>().Property(_ => _.Text).HasMaxLength(30);

            // Primary Key
            builder.Entity<ImputationNumber>().HasKey(_ => _.Id);
            builder.Entity<ImputationNumber>().Property(_ => _.Text).HasMaxLength(10);

            // Primary Key
            builder.Entity<Currency>().HasKey(_ => _.Id);
            builder.Entity<Currency>().Property(_ => _.Text).HasMaxLength(15);
        }
    }
}
