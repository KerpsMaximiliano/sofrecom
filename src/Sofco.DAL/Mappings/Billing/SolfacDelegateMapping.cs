using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Mappings.Billing
{
    public static class SolfacDelegateMapping
    {
        public static void MapSolfacDelegate(this ModelBuilder builder)
        {
            builder.Entity<SolfacDelegate>().HasKey(_ => _.Id);
            builder.Entity<SolfacDelegate>().Property(_ => _.ServiceId).IsRequired();
            builder.Entity<SolfacDelegate>().Property(_ => _.UserId).IsRequired();
            builder.Entity<SolfacDelegate>().Property(_ => _.CreatedUser).HasMaxLength(50);
            builder.Entity<SolfacDelegate>().HasIndex(x => new { x.ServiceId, x.UserId }).IsUnique();
        }
    }
}
