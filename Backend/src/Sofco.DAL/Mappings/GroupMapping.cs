using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Mappings
{
    public static class GroupMapping
    {
        public static void MapGroups(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Group>().HasKey(_ => _.Id);
            builder.Entity<Group>().Property(_ => _.Description).HasMaxLength(50).IsRequired();
        }
    }
}
