using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class HealthInsuranceMapping
    {
        public static void MapHealthInsurance(this ModelBuilder builder)
        {
            builder.Entity<HealthInsurance>().HasKey(x => x.Id);
            builder.Entity<HealthInsurance>().HasIndex(x => x.Code).IsUnique();
            builder.Entity<HealthInsurance>().Property(x => x.Name).HasMaxLength(400);
        }
    }
}
