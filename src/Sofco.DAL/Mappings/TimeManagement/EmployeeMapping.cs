using Microsoft.EntityFrameworkCore;
using Sofco.Model.Models.TimeManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sofco.DAL.Mappings.TimeManagement
{
    public static class EmployeeMapping
    {
        public static void MapEmployee(this ModelBuilder builder)
        {
            builder.Entity<Employee>().HasKey(_ => _.Id);
            builder.Entity<Employee>().Property(x => x.Name).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Profile).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Seniority).HasMaxLength(100);
            builder.Entity<Employee>().Property(x => x.Technology).HasMaxLength(300);
            builder.Entity<Employee>().Property(x => x.EmployeeNumber).HasMaxLength(50);
        }
    }
}
