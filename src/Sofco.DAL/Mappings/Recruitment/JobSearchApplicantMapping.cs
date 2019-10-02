using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.DAL.Mappings.Recruitment
{
    public static class JobSearchApplicantMapping
    {
        public static void MapJobSearchApplicant(this ModelBuilder builder)
        {
            builder.Entity<JobSearchApplicant>().HasKey(t => new { t.JobSearchId, t.ApplicantId });

            builder.Entity<JobSearchApplicant>()
                .HasOne(pt => pt.JobSearch)
                .WithMany(p => p.JobSearchApplicants)
                .HasForeignKey(pt => pt.JobSearchId);

            builder.Entity<JobSearchApplicant>()
                .HasOne(pt => pt.Applicant)
                .WithMany(p => p.JobSearchApplicants)
                .HasForeignKey(pt => pt.ApplicantId);

            builder.Entity<JobSearchApplicant>().Property(x => x.CreatedBy).HasMaxLength(50);
            builder.Entity<JobSearchApplicant>().Property(x => x.Comments).HasMaxLength(3000);

            builder.Entity<JobSearchApplicant>().HasOne(x => x.Reason)
                .WithMany(x => x.JobSearchApplicants)
                .HasForeignKey(x => x.ReasonId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
