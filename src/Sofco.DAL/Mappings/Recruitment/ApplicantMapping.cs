using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Relationships;

namespace Sofco.DAL.Mappings.Recruitment
{
    public static class ApplicantMapping
    {
        public static void MapApplicant(this ModelBuilder builder)
        {
            builder.Entity<Applicant>().HasKey(x => x.Id);
            builder.Entity<Applicant>().Property(x => x.Comments).HasMaxLength(3000);
            builder.Entity<Applicant>().Property(x => x.Email).HasMaxLength(75);
            builder.Entity<Applicant>().Property(x => x.LastName).HasMaxLength(75);
            builder.Entity<Applicant>().Property(x => x.FirstName).HasMaxLength(75);
            builder.Entity<Applicant>().Property(x => x.CountryCode1).HasMaxLength(5);
            builder.Entity<Applicant>().Property(x => x.AreaCode1).HasMaxLength(5);
            builder.Entity<Applicant>().Property(x => x.Telephone1).HasMaxLength(15);
            builder.Entity<Applicant>().Property(x => x.CountryCode2).HasMaxLength(5);
            builder.Entity<Applicant>().Property(x => x.AreaCode2).HasMaxLength(5);
            builder.Entity<Applicant>().Property(x => x.Telephone2).HasMaxLength(15);
            builder.Entity<Applicant>().Property(x => x.CreatedBy).HasMaxLength(25);
            builder.Entity<Applicant>().Property(x => x.DocumentNumber).HasMaxLength(10);
            builder.Entity<Applicant>().Property(x => x.Nationality).HasMaxLength(50);
            builder.Entity<Applicant>().Property(x => x.CivilStatus).HasMaxLength(50);
            builder.Entity<Applicant>().Property(x => x.Address).HasMaxLength(100);
            builder.Entity<Applicant>().Property(x => x.Cuil).HasMaxLength(12);
            builder.Entity<Applicant>().Property(x => x.Prepaid).HasMaxLength(100);
            builder.Entity<Applicant>().Property(x => x.Office).HasMaxLength(100);
            builder.Entity<Applicant>().Property(x => x.Aggreements).HasMaxLength(3000);

            builder.Entity<Applicant>().HasOne(x => x.Client).WithMany(x => x.Applicants).HasForeignKey(x => x.ClientId);
            builder.Entity<Applicant>().HasOne(x => x.ReasonCause).WithMany(x => x.Applicants).HasForeignKey(x => x.ReasonCauseId);
            builder.Entity<Applicant>().HasOne(x => x.RecommendedByUser).WithMany(x => x.Applicants).HasForeignKey(x => x.RecommendedByUserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Applicant>().HasOne(x => x.Manager).WithMany(x => x.Applicants2).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Applicant>().HasOne(x => x.Analytic).WithMany(x => x.Applicants).HasForeignKey(x => x.AnalyticId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Applicant>().HasOne(x => x.Project).WithMany(x => x.Applicants).HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Applicant>().HasOne(x => x.Profile).WithMany(x => x.Applicants).HasForeignKey(x => x.ProfileId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicantProfile>().HasKey(t => new { t.ApplicantId, t.ProfileId });

            builder.Entity<ApplicantProfile>()
                .HasOne(pt => pt.Applicant)
                .WithMany(p => p.ApplicantProfiles)
                .HasForeignKey(pt => pt.ApplicantId);

            builder.Entity<ApplicantProfile>()
                .HasOne(pt => pt.Profile)
                .WithMany(p => p.ApplicantProfiles)
                .HasForeignKey(pt => pt.ProfileId);

            builder.Entity<ApplicantSkills>().HasKey(t => new { t.ApplicantId, t.SkillId });

            builder.Entity<ApplicantSkills>()
                .HasOne(pt => pt.Applicant)
                .WithMany(p => p.ApplicantSkills)
                .HasForeignKey(pt => pt.ApplicantId);

            builder.Entity<ApplicantSkills>()
                .HasOne(pt => pt.Skill)
                .WithMany(p => p.ApplicantProfiles)
                .HasForeignKey(pt => pt.SkillId);

            builder.Entity<ApplicantHistory>().HasKey(x => x.Id);
            builder.Entity<ApplicantHistory>().Property(x => x.Comment).HasMaxLength(1000);
            builder.Entity<ApplicantHistory>().Property(x => x.UserName).HasMaxLength(50);
            builder.Entity<ApplicantHistory>().HasOne(x => x.ReasonCause).WithMany(x => x.ApplicantHistories).HasForeignKey(x => x.ReasonCauseId);
        }
    }
}
