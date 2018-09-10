using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.DAL.Mappings.Billing
{
    public static class CloseDateMapping
    {
        public static void MapCloseDate(this ModelBuilder builder)
        {
            builder.Entity<CloseDate>().HasKey(_ => _.Id);
        }
    }
}
