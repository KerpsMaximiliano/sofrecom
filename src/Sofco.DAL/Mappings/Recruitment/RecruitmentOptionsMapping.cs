using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.DAL.Mappings.Recruitment
{
    public static class RecruitmentOptionsMapping
    {
        public static void MapRecruitmentOptions(this ModelBuilder builder)
        {
            // Primary Key
            builder.Entity<Seniority>().HasKey(_ => _.Id);
            builder.Entity<Seniority>().Property(_ => _.Text).HasMaxLength(75);

            // Primary Key
            builder.Entity<Profile>().HasKey(_ => _.Id);
            builder.Entity<Profile>().Property(_ => _.Text).HasMaxLength(75);

            // Primary Key
            builder.Entity<Skill>().HasKey(_ => _.Id);
            builder.Entity<Skill>().Property(_ => _.Text).HasMaxLength(75);

            // Primary Key
            builder.Entity<ReasonCause>().HasKey(_ => _.Id);
            builder.Entity<ReasonCause>().Property(_ => _.Text).HasMaxLength(75);

            // Primary Key
            builder.Entity<TimeHiring>().HasKey(_ => _.Id);
            builder.Entity<TimeHiring>().Property(_ => _.Text).HasMaxLength(75);

            // Primary Key
            builder.Entity<ResourceAssignment>().HasKey(_ => _.Id);
            builder.Entity<ResourceAssignment>().Property(_ => _.Text).HasMaxLength(75);
        }
    }
}
