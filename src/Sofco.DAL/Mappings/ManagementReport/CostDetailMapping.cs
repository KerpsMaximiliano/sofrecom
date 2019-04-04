using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Mappings.ManagementReport
{
    public static class CostDetailMapping
    {
        public static void MapCostDetail(this ModelBuilder builder)
        {
            builder.Entity<CostDetail>().HasKey(x => x.Id);

            builder.Entity<CostDetail>().HasOne(x => x.Analytic).WithMany(x => x.CostDetail).HasForeignKey(x => x.IdAnalytic).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CostDetail>().HasOne(r => r.Type).WithMany(t => t.CostDetail).HasForeignKey(r => r.TypeId);
            builder.Entity<CostDetail>().HasOne(h => h.Employee).WithMany(e => e.CostDetail).HasForeignKey(r => r.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CostDetail>().HasOne(r => r.CreatedBy).WithMany(u => u.CostDetail2).HasForeignKey(r => r.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CostDetail>().HasOne(x => x.ModifiedBy).WithMany(u => u.CostDetail3).HasForeignKey(r => r.ModifiedById).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CostDetailResourceType>().HasKey(h => h.Id);
            builder.Entity<CostDetailResourceType>().Property(x => x.Name).HasMaxLength(250);
        }
    }
}
