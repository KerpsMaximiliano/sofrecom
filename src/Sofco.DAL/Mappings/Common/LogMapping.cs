using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Mappings.Common
{
    public static class LogMapping
    {
        public static void MapLog(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Log>().HasKey(_ => _.Id);
            builder.Entity<Log>().Property(_ => _.Username).HasMaxLength(50);
            builder.Entity<Log>().Property(_ => _.Comment).HasMaxLength(1000);
        }
    }
}
