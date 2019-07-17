using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.DAL.Mappings.AllocationManagement
{
    public static class EmployeeMapping
    {
        public static void MapEmployee(this ModelBuilder builder)
        {
            builder.Entity<Employee>().HasKey(_ => _.Id);
            builder.Entity<Employee>().HasIndex(u => u.EmployeeNumber).IsUnique();
            builder.Entity<Employee>().Property(x => x.Name).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Profile).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Seniority).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Technology).HasMaxLength(300);
            builder.Entity<Employee>().Property(x => x.CreatedByUser).HasMaxLength(50);
            builder.Entity<Employee>().Property(x => x.Address).HasMaxLength(400);
            builder.Entity<Employee>().Property(x => x.Location).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.Province).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.Country).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.OfficeAddress).HasMaxLength(400);
            builder.Entity<Employee>().Property(x => x.Email).HasMaxLength(150);
            builder.Entity<Employee>().Property(x => x.Salary).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.PrepaidAmount).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.BusinessHoursDescription).HasMaxLength(150);
            builder.Entity<Employee>().Property(x => x.EndReason).HasMaxLength(2000);
            builder.Entity<Employee>().Property(x => x.DocumentNumberType).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.PrepaidPlan).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.PhoneNumber).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Bank).HasMaxLength(200);
            builder.Entity<Employee>().Property(x => x.Cuil).HasColumnType("decimal(12, 0)");

            builder.Entity<Employee>().HasMany(x => x.Licenses).WithOne(x => x.Employee).HasForeignKey(x => x.EmployeeId);
            builder.Entity<Employee>().HasOne(x => x.TypeEndReason).WithMany(x => x.Employees).HasForeignKey(x => x.TypeEndReasonId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Employee>().HasOne(x => x.Manager).WithMany(x => x.Employees).HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.SetNull);


            builder.Entity<SocialCharge>().HasKey(_ => _.Id);
            builder.Entity<SocialCharge>().HasOne(x => x.Employee).WithMany(x => x.SocialCharges).HasForeignKey(x => x.EmployeeId);
            builder.Entity<SocialCharge>().HasMany(x => x.Items).WithOne(x => x.SocialCharge).HasForeignKey(x => x.SocialChargeId);

            builder.Entity<SocialChargeItem>().HasKey(_ => _.Id);
            builder.Entity<SocialChargeItem>().Property(x => x.AccountName).HasMaxLength(500);
            builder.Entity<SocialChargeItem>().HasOne(x => x.SocialCharge).WithMany(x => x.Items).HasForeignKey(x => x.SocialChargeId);
        }
    }
}
