using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Mappings.Recruitment
{
    public static class JobSearchMapping
    {
        public static void MapJobSearch(this ModelBuilder builder)
        {
            builder.Entity<JobSearch>().HasKey(x => x.Id);
            builder.Entity<JobSearch>().Property(x => x.Comments).HasMaxLength(3000);
            builder.Entity<JobSearch>().Property(x => x.TimeHiring).HasMaxLength(100);
            builder.Entity<JobSearch>().Property(x => x.CreatedBy).HasMaxLength(50);

            builder.Entity<JobSearch>().HasOne(x => x.Client).WithMany(x => x.JobSearchs).HasForeignKey(x => x.ClientId);
            builder.Entity<JobSearch>().HasOne(x => x.ReasonCause).WithMany(x => x.JobSearchs).HasForeignKey(x => x.ReasonCauseId);
            builder.Entity<JobSearch>().HasOne(x => x.User).WithMany(x => x.JobSearchs).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<JobSearch>().HasOne(x => x.Recruiter).WithMany(x => x.JobSearchs2).HasForeignKey(x => x.RecruiterId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<JobSearchProfile>().HasKey(t => new { t.JobSearchId, t.ProfileId });

            builder.Entity<JobSearchProfile>()
                .HasOne(pt => pt.JobSearch)
                .WithMany(p => p.JobSearchProfiles)
                .HasForeignKey(pt => pt.JobSearchId);

            builder.Entity<JobSearchProfile>()
                .HasOne(pt => pt.Profile)
                .WithMany(p => p.JobSearchProfiles)
                .HasForeignKey(pt => pt.ProfileId);

            builder.Entity<JobSearchSeniority>().HasKey(t => new { t.JobSearchId, t.SeniorityId });

            builder.Entity<JobSearchSeniority>()
                .HasOne(pt => pt.JobSearch)
                .WithMany(p => p.JobSearchSeniorities)
                .HasForeignKey(pt => pt.JobSearchId);

            builder.Entity<JobSearchSeniority>()
                .HasOne(pt => pt.Seniority)
                .WithMany(p => p.JobSearchSeniorities)
                .HasForeignKey(pt => pt.SeniorityId);

            builder.Entity<JobSearchSkill>().HasKey(t => new { t.JobSearchId, t.SkillId });

            builder.Entity<JobSearchSkill>()
                .HasOne(pt => pt.JobSearch)
                .WithMany(p => p.JobSearchSkills)
                .HasForeignKey(pt => pt.JobSearchId);

            builder.Entity<JobSearchSkill>()
                .HasOne(pt => pt.Skill)
                .WithMany(p => p.JobSearchSkills)
                .HasForeignKey(pt => pt.SkillId);
        }
    }
}
