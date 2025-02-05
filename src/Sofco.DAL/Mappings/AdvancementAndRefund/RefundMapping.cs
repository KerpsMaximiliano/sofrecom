﻿using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Mappings.AdvancementAndRefund
{
    public static class RefundMapping
    {
        public static void MapRefund(this ModelBuilder builder)
        {
            builder.Entity<Refund>().HasKey(t => t.Id);

            builder.Entity<Refund>().HasOne(x => x.UserApplicant).WithMany(x => x.Refunds).HasForeignKey(x => x.UserApplicantId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Refund>().HasOne(x => x.Analytic).WithMany(x => x.Refunds).HasForeignKey(x => x.AnalyticId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Refund>().HasOne(x => x.Currency).WithMany(x => x.Refunds).HasForeignKey(x => x.CurrencyId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Refund>().HasOne(x => x.Workflow).WithMany(x => x.Refunds).HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Refund>().HasOne(x => x.Status).WithMany(x => x.Refunds).HasForeignKey(x => x.StatusId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Refund>().HasMany(x => x.Histories).WithOne(x => x.Refund).HasForeignKey(x => x.RefundId);
            builder.Entity<Refund>().HasMany(x => x.Details).WithOne(x => x.Refund).HasForeignKey(x => x.RefundId);

            builder.Entity<Refund>().HasMany(x => x.Attachments).WithOne(x => x.Refund).HasForeignKey(x => x.RefundId);

            builder.Entity<RefundHistory>().HasKey(x => x.Id);
            builder.Entity<RefundHistory>().Property(x => x.Comment).HasMaxLength(1000);
            builder.Entity<RefundHistory>().Property(x => x.UserName).HasMaxLength(100);

            builder.Entity<RefundHistory>().HasOne(x => x.StatusFrom).WithMany(x => x.RefundHistories).HasForeignKey(x => x.StatusFromId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<RefundHistory>().HasOne(x => x.StatusTo).WithMany(x => x.RefundHistories2).HasForeignKey(x => x.StatusToId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RefundHistory>().HasOne(x => x.Refund).WithMany(x => x.Histories).HasForeignKey(x => x.RefundId);

            builder.Entity<RefundDetail>().HasKey(x => x.Id);
            builder.Entity<RefundDetail>().Property(x => x.Description).HasMaxLength(300);
            builder.Entity<RefundDetail>().HasOne(x => x.Refund).WithMany(x => x.Details).HasForeignKey(x => x.RefundId);
            builder.Entity<RefundDetail>().HasOne(x => x.CostType).WithMany(x => x.RefundDetails).HasForeignKey(x => x.CostTypeId);

            builder.Entity<RefundFile>().HasKey(t => new { t.FileId, t.RefundId });

            builder.Entity<RefundFile>()
                .HasOne(pt => pt.Refund)
                .WithMany(p => p.Attachments)
                .HasForeignKey(pt => pt.RefundId);

            builder.Entity<RefundFile>()
                .HasOne(pt => pt.File)
                .WithMany()
                .HasForeignKey(pt => pt.FileId);

            builder.Entity<CostType>().HasKey(x => x.Id);
            builder.Entity<CostType>().Property(x => x.Text).HasMaxLength(200);
        }
    }
}
