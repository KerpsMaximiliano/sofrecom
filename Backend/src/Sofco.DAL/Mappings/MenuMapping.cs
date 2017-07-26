using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models;

namespace Sofco.DAL.Mapping
{
    public static class MenuMapping
    {
        public static void MapMenus(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Menu>().HasKey(_ => _.Id);
            builder.Entity<Menu>().Property(_ => _.Description).HasMaxLength(50);
            builder.Entity<Menu>().Property(_ => _.Code).HasMaxLength(4);
            builder.Entity<Menu>().Property(_ => _.Url).HasMaxLength(200);
        }
    }
}
