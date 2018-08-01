using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Mappings.Admin
{
    public static class UserGroupMapping
    {
        public static void MapUserGroups(this ModelBuilder builder)
        {
            builder.Entity<UserGroup>().HasKey(t => new { t.UserId, t.GroupId });

            builder.Entity<UserGroup>()
                 .HasOne(pt => pt.User)
                 .WithMany(p => p.UserGroups)
                 .HasForeignKey(pt => pt.UserId);

            builder.Entity<UserGroup>()
                .HasOne(pt => pt.Group)
                .WithMany(t => t.UserGroups)
                .HasForeignKey(pt => pt.GroupId);
        }
    }
}
