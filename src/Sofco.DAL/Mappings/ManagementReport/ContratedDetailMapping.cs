using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.DAL.Mappings.ManagementReport
{
    public static class ContratedDetailMapping
    {
        public static void MapContratedDetail(this ModelBuilder builder)
        {
            builder.Entity<ContratedDetail>().HasKey(x => x.Id);

            builder.Entity<ContratedDetail>().Property(x => x.Name).HasMaxLength(250);

            builder.Entity<ContratedDetail>().HasOne(x => x.Analytic).WithMany(x => x.ContratedDetail).HasForeignKey(x => x.IdAnalytic).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
