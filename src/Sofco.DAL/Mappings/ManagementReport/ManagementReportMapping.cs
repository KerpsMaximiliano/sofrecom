using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.ManagementReport
{
    public static class ManagementReportMapping
    {
        public static void MapManagementReport(this ModelBuilder builder)
        {
            builder.Entity<Domain.Models.ManagementReport.ManagementReport>().HasKey(x => x.Id);

            builder.Entity<Analytic>().HasOne(x => x.ManagementReport).WithOne(x => x.Analytic)
                .HasForeignKey<Domain.Models.ManagementReport.ManagementReport>(x => x.AnalyticId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
