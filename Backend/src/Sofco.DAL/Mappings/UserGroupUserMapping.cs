using Microsoft.EntityFrameworkCore;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Mapping
{
    public static class UserGroupUserMapping
    {
        public static void MapUserGroupsUser(this ModelBuilder builder)
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
