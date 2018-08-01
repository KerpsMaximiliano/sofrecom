using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Mappings.Common
{
    public static class UserDelegateMapping
    {
        public static void MapUserDelegate(this ModelBuilder builder)
        {
            builder.Entity<UserDelegate>().HasKey(_ => _.Id);
            builder.Entity<UserDelegate>().Property(_ => _.ServiceId).IsRequired();
            builder.Entity<UserDelegate>().Property(_ => _.UserId).IsRequired();
            builder.Entity<UserDelegate>().Property(_ => _.CreatedUser).HasMaxLength(50);
            builder.Entity<UserDelegate>().HasIndex(x => new { x.ServiceId, x.UserId }).IsUnique();
        }
    }
}
