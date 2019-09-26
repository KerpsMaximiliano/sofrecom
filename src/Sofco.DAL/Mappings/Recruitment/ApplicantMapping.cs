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

            builder.Entity<Applicant>().HasOne(x => x.Client).WithMany(x => x.Applicants).HasForeignKey(x => x.ClientId);
            builder.Entity<Applicant>().HasOne(x => x.ReasonCause).WithMany(x => x.Applicants).HasForeignKey(x => x.ReasonCauseId);
            builder.Entity<Applicant>().HasOne(x => x.RecommendedByUser).WithMany(x => x.Applicants).HasForeignKey(x => x.RecommendedByUserId).OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}
