using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Mappings
{
    public static class MenuMapping
    {
        public static void MapMenus(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Menu>().HasKey(_ => _.Id);
            builder.Entity<Menu>().Property(_ => _.Description).HasMaxLength(50).IsRequired();
            builder.Entity<Menu>().Property(_ => _.Code).HasMaxLength(5).IsRequired();
            builder.Entity<Menu>().Property(_ => _.Url).HasMaxLength(200).IsRequired();
        }
    }
}
