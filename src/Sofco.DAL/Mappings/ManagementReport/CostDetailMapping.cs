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
            builder.Entity<CostDetail>().HasMany(cd => cd.Employees).WithOne(e => e.CostDetail).HasForeignKey(e => e.CostDetailId);
            builder.Entity<CostDetail>().HasMany(cd => cd.Resources).WithOne(r => r.CostDetail).HasForeignKey(r => r.CostDetailId);

            builder.Entity<CostDetailResource>().HasKey(r => r.Id);
            builder.Entity<CostDetailResource>().HasOne(r => r.Type).WithMany(t => t.Resources).HasForeignKey(r => r.TypeId);
            builder.Entity<CostDetailResource>().HasOne(r => r.CreatedBy).WithMany(u => u.CostDetailResource).HasForeignKey(r => r.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CostDetailResource>().HasOne(x => x.ModifiedBy).WithMany(u => u.CostDetailResource2).HasForeignKey(r => r.ModifiedById).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CostDetailHumanResource>().HasKey(h => h.Id);
            builder.Entity<CostDetailHumanResource>().HasOne(h => h.Employee).WithMany(e => e.CostDetailHumanResources).HasForeignKey(r => r.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CostDetailHumanResource>().HasOne(r => r.User).WithMany(u => u.CostDetailHumanResources).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CostDetailHumanResource>().HasOne(r => r.CreatedBy).WithMany(u => u.CostDetailHumanResources2).HasForeignKey(r => r.CreatedById).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CostDetailHumanResource>().HasOne(x => x.ModifiedBy).WithMany(u => u.CostDetailHumanResources3).HasForeignKey(r => r.ModifiedById).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CostDetailResourceType>().HasKey(h => h.Id);
            builder.Entity<CostDetailResourceType>().Property(x => x.Name).HasMaxLength(250);
        }
    }
}
