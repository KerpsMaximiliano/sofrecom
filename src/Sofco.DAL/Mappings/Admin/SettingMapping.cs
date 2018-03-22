using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class SettingMapping
    {
        public static void MapSetting(this ModelBuilder builder)
        {
            builder.Entity<Setting>().HasKey(_ => _.Id);
            builder.Entity<Setting>().Property(_ => _.Key).HasMaxLength(100).IsRequired();
            builder.Entity<Setting>().Property(_ => _.Value).HasMaxLength(500).IsRequired();
        }
    }
}
