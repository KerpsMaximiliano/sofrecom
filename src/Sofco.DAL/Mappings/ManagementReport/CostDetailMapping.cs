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

            //builder.Entity<CostDetailType>().HasKey(h => h.Id);
            //builder.Entity<CostDetailType>().Property(x => x.Name).HasMaxLength(250);

            //builder.Entity<CostDetailSubtype>().HasKey(h => h.Id);
            //builder.Entity<CostDetailSubtype>().Property(x => x.Name).HasMaxLength(250);
            //builder.Entity<CostDetailSubtype>().HasOne(x => x.CostDetailType).WithMany(x => x.CostDetailSubtype).HasForeignKey(x => x.CostDetailTypeId);

            //Cost detail categories
            builder.Entity<CostDetailCategories>().HasKey(h => h.Id);
            builder.Entity<CostDetailCategories>().Property(x => x.Name).HasMaxLength(250);

            //Cost detail subcategories
            builder.Entity<CostDetailSubcategories>().HasKey(h => h.Id);
            builder.Entity<CostDetailSubcategories>().Property(x => x.Name).HasMaxLength(250);
            builder.Entity<CostDetailSubcategories>().HasOne(x => x.CostDetailCategory).WithMany(x => x.Subcategories).HasForeignKey(x => x.CostDetailCategoryId);

        }
    }
}
