﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Extensions;

namespace Sofco.Repository.Rh.Maps
{
    public class RhproEmployeeLicenseMap : EntityMappingConfiguration<RhproEmployeeLicense>
    {
        const string TableName = "emp_lic";

        public override void Map(EntityTypeBuilder<RhproEmployeeLicense> builder)
        {
            builder.ToTable(TableName);

            builder.HasKey(s => s.Empleado);
        }
    }
}
