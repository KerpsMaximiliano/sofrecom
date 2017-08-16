using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Mapping
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
