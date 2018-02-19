using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class HealthInsuranceMapping
    {
        public static void MapHealthInsurance(this ModelBuilder builder)
        {
            builder.Entity<HealthInsurance>().HasKey(_ => _.Id);
            builder.Entity<HealthInsurance>().HasIndex(u => u.Code).IsUnique();
            builder.Entity<HealthInsurance>().Property(x => x.Name).HasMaxLength(400);
        }
    }
}
