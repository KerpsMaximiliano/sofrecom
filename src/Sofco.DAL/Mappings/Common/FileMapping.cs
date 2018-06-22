using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Common;

namespace Sofco.DAL.Mappings.Common
{
    public static class FileMapping
    {
        public static void MapFile(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<File>().HasKey(_ => _.Id);
            builder.Entity<File>().Property(_ => _.FileName).HasMaxLength(500);
            builder.Entity<File>().Property(_ => _.InternalFileName).HasMaxLength(100);
            builder.Entity<File>().Property(_ => _.FileType).HasMaxLength(10);
            builder.Entity<File>().Property(_ => _.CreatedUser).HasMaxLength(50);
        }
    }
}
