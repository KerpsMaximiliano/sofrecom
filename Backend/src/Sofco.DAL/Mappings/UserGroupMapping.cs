using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Mapping
{
    public static class UserGroupMapping
    {
        public static void MapUserGroups(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<UserGroup>().HasKey(_ => _.Id);
            builder.Entity<UserGroup>().Property(_ => _.Description).HasMaxLength(50);
        }
    }
}
