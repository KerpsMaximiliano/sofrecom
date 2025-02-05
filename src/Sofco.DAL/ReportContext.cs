﻿using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.Reports;

namespace Sofco.DAL
{
    public class ReportContext : DbContext
    {
        public const string SchemaName = "report";

        public ReportContext(DbContextOptions<ReportContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(SchemaName);

            builder.Entity<PurchaseOrderBalanceView>().ToTable("PurchaseOrderBalanceView");

            builder.Entity<PurchaseOrderBalanceDetailView>().ToTable("PurchaseOrderBalanceDetailView");

            builder.Entity<BankView>().ToTable("BanksView");

            builder.Entity<EmployeeView>().ToTable("ResourcesView");
        }
    }
}
