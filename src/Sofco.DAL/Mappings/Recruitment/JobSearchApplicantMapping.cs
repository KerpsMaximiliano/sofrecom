using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.DAL.Mappings.Recruitment
{
    public static class JobSearchApplicantMapping
    {
        public static void MapJobSearchApplicant(this ModelBuilder builder)
        {
            builder.Entity<JobSearchApplicant>().HasKey(t => new { t.JobSearchId, t.ApplicantId, t.CreatedDate });
            builder.Entity<JobSearchApplicant>().Property(x => x.RrhhInterviewPlace).HasMaxLength(100);
            builder.Entity<JobSearchApplicant>().Property(x => x.TechnicalInterviewPlace).HasMaxLength(100);
            builder.Entity<JobSearchApplicant>().Property(x => x.ClientInterviewPlace).HasMaxLength(100);
            builder.Entity<JobSearchApplicant>().Property(x => x.RrhhInterviewComments).HasMaxLength(100);
            builder.Entity<JobSearchApplicant>().Property(x => x.TechnicalInterviewComments).HasMaxLength(100);
            builder.Entity<JobSearchApplicant>().Property(x => x.ClientInterviewComments).HasMaxLength(100);

            builder.Entity<JobSearchApplicant>()
                .HasOne(pt => pt.RrhhInterviewer)
                .WithMany(p => p.JobSearchApplicants1)
                .HasForeignKey(pt => pt.RrhhInterviewerId);

            builder.Entity<JobSearchApplicant>()
                .HasOne(pt => pt.TechnicalInterviewer)
                .WithMany(p => p.JobSearchApplicants2)
                .HasForeignKey(pt => pt.TechnicalInterviewerId);

            builder.Entity<JobSearchApplicant>()
                .HasOne(pt => pt.ClientInterviewer)
                .WithMany(p => p.JobSearchApplicants3)
                .HasForeignKey(pt => pt.ClientInterviewerId);

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
