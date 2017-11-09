using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Mappings.Admin
{
    public static class GlobalSettingMapping
    {
        public static void MapGlobalSetting(this ModelBuilder builder)
        {
            builder.Entity<GlobalSetting>().HasKey(_ => _.Id);
            builder.Entity<GlobalSetting>().Property(_ => _.Key).HasMaxLength(100).IsRequired();
            builder.Entity<GlobalSetting>().Property(_ => _.Value).HasMaxLength(500).IsRequired();
        }
    }
}
