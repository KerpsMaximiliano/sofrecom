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

            builder.Entity<CostDetailType>().HasKey(h => h.Id);
            builder.Entity<CostDetailType>().Property(x => x.Name).HasMaxLength(250);
        }
    }
}
