﻿using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.DAL.Mappings.ManagementReport
{
    public static class ManagementReportMapping
    {
        public static void MapManagementReport(this ModelBuilder builder)
        {
            // Management Report
            builder.Entity<Domain.Models.ManagementReport.ManagementReport>().HasKey(x => x.Id);

            builder.Entity<Analytic>().HasOne(x => x.ManagementReport).WithOne(x => x.Analytic)
                .HasForeignKey<Domain.Models.ManagementReport.ManagementReport>(x => x.AnalyticId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Domain.Models.ManagementReport.ManagementReport>().HasMany(x => x.CostDetails)
                .WithOne(x => x.ManagementReport).HasForeignKey(x => x.ManagementReportId);

            builder.Entity<Domain.Models.ManagementReport.ManagementReport>().HasMany(x => x.Billings)
                .WithOne(x => x.ManagementReport).HasForeignKey(x => x.ManagementReportId);

            builder.Entity<Domain.Models.ManagementReport.ManagementReport>().HasMany(x => x.Budgets)
                .WithOne(x => x.ManagementReport).HasForeignKey(x => x.ManagementReportId);

            builder.Entity<Domain.Models.ManagementReport.ManagementReport>().HasOne(x => x.State)
                .WithMany(x => x.ManagementReport).HasForeignKey(x => x.StateId);


            // Cost Detail
            builder.Entity<CostDetail>().HasKey(x => x.Id);
            builder.Entity<CostDetail>().HasOne(x => x.ManagementReport).WithMany(x => x.CostDetails).HasForeignKey(x => x.ManagementReportId);
            builder.Entity<CostDetail>().HasMany(x => x.CostDetailResources).WithOne(x => x.CostDetail).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetail>().HasMany(x => x.CostDetailProfiles).WithOne(x => x.CostDetail).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetail>().HasMany(x => x.CostDetailOthers).WithOne(x => x.CostDetail).HasForeignKey(x => x.CostDetailId);

            // Cost Detail Resource
            builder.Entity<CostDetailResource>().HasKey(x => x.Id);
            builder.Entity<CostDetailResource>().HasIndex(x => new { x.CostDetailId, x.EmployeeId, x.BudgetTypeId }).IsUnique();
            builder.Entity<CostDetailResource>().HasOne(x => x.CostDetail).WithMany(x => x.CostDetailResources).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetailResource>().HasOne(x => x.Employee).WithMany(x => x.CostDetailResources).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetailResource>().HasOne(x => x.User).WithMany(x => x.CostDetailResources).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetailResource>().HasOne(x => x.BudgetType).WithMany(x => x.CostDetailResource).HasForeignKey(x => x.BudgetTypeId);

            // Cost Detail Profile
            builder.Entity<CostDetailProfile>().HasKey(x => x.Id);
            builder.Entity<CostDetailProfile>().Property(x => x.Description).HasMaxLength(500);
            builder.Entity<CostDetailProfile>().HasOne(x => x.CostDetail).WithMany(x => x.CostDetailProfiles).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetailProfile>().HasOne(x => x.EmployeeProfile).WithMany(x => x.CostDetailProfiles).HasForeignKey(x => x.EmployeeProfileId);

            // Cost Detail Other
            builder.Entity<CostDetailOther>().HasKey(x => x.Id);
            builder.Entity<CostDetailOther>().Property(x => x.Description).HasMaxLength(500);
            builder.Entity<CostDetailOther>().HasOne(x => x.CostDetail).WithMany(x => x.CostDetailOthers).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetailOther>().HasOne(x => x.CostDetailSubcategory).WithMany(x => x.CostDetailOther).HasForeignKey(x => x.CostDetailSubcategoryId);
            //builder.Entity<CostDetailOther>().HasOne(x => x).WithMany(x => x.CostDetailOthers).HasForeignKey(x => x.CostDetailSubtypeId);
            //builder.Entity<CostDetailOther>().HasOne(x => x.CostDetailType).WithMany(x => x.CostDetailOthers).HasForeignKey(x => x.CostDetailTypeId);

            // Billing
            builder.Entity<ManagementReportBilling>().HasKey(x => x.Id);
            builder.Entity<ManagementReportBilling>().Property(x => x.Comments).HasMaxLength(2000);
            builder.Entity<ManagementReportBilling>().HasOne(x => x.ManagementReport).WithMany(x => x.Billings).HasForeignKey(x => x.ManagementReportId);

            // Budget
            builder.Entity<Budget>().HasKey(x => x.Id);
            builder.Entity<Budget>().Property(x => x.Description).HasMaxLength(200);
            builder.Entity<Budget>().Property(x => x.ModifiedBy).HasMaxLength(100);
            builder.Entity<Budget>().HasOne(x => x.ManagementReport).WithMany(x => x.Budgets).HasForeignKey(x => x.ManagementReportId);

            // Type Budget
            builder.Entity<BudgetType>().HasKey(x => x.Id);
            builder.Entity<BudgetType>().Property(x => x.Name).HasMaxLength(250);

            // Cost Detail Other
            builder.Entity<CostDetailStaff>().HasKey(x => x.Id);
            builder.Entity<CostDetailStaff>().Property(x => x.Description).HasMaxLength(500);
            builder.Entity<CostDetailStaff>().HasOne(x => x.CostDetail).WithMany(x => x.CostDetailStaff).HasForeignKey(x => x.CostDetailId);
            builder.Entity<CostDetailStaff>().HasOne(x => x.CostDetailSubcategory).WithMany(x => x.CostDetailStaff).HasForeignKey(x => x.CostDetailSubcategoryId);
            builder.Entity<CostDetailStaff>().HasOne(x => x.BudgetType).WithMany(x => x.CostDetailStaff).HasForeignKey(x => x.BudgetTypeId);

            builder.Entity<ManagementReportComment>().HasKey(x => x.Id);
            builder.Entity<ManagementReportComment>().Property(x => x.Comment).HasMaxLength(2000);
            builder.Entity<ManagementReportComment>().HasOne(x => x.ManagementReport).WithMany(x => x.Comments).HasForeignKey(x => x.ManagementReportId);

            builder.Entity<ResourceBilling>().HasKey(x => x.Id);
            builder.Entity<ResourceBilling>().HasOne(x => x.Seniority).WithMany(x => x.ResourceBillings).HasForeignKey(x => x.SeniorityId);
            builder.Entity<ResourceBilling>().HasOne(x => x.Employee).WithMany(x => x.ResourceBillings).HasForeignKey(x => x.EmployeeId);
            builder.Entity<ResourceBilling>().HasOne(x => x.ManagementReportBilling).WithMany(x => x.ResourceBillings).HasForeignKey(x => x.ManagementReportBillingId);
        }
    }
}
