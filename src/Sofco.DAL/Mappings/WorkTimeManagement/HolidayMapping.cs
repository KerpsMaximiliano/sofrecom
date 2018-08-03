using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.DAL.Mappings.WorkTimeManagement
{
    public static class HolidayMapping
    {
        public static void MapHoliday(this ModelBuilder builder)
        {
            builder.Entity<Holiday>().HasKey(_ => _.Id);

            builder.Entity<Holiday>().Property(x => x.DataSource).HasMaxLength(50);
            builder.Entity<Holiday>().Property(x => x.Name).HasMaxLength(500);
        }
    }
}
