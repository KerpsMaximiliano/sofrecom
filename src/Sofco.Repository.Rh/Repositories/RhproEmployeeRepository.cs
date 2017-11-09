using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Rhpro;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Repository.Rh.Repositories
{
    public class RhproEmployeeRepository : IRhproEmployeeRepository
    {
        private DbSet<RhproEmployee> RhproEmployeeSet { get; }

        private DbSet<RhproEmployeeLicense> RhproEmployeeLicenseSet { get; }

        private DbSet<RhproLicenseType> RhproLicenseTypeSet { get; }

        public RhproEmployeeRepository(RhproContext context)
        {
            RhproEmployeeSet = context.Set<RhproEmployee>();

            RhproEmployeeLicenseSet = context.Set<RhproEmployeeLicense>();

            RhproLicenseTypeSet = context.Set<RhproLicenseType>();
        }

        public IList<RhproEmployee> GetAll()
        {
            return RhproEmployeeSet.ToList();
        }

        public IList<RhproLicenseType> GetLicenseTypes()
        {
            return RhproLicenseTypeSet.ToList();
        }

        public IList<RhproEmployeeLicense> GetEmployeeLicensesWithStartDate(DateTime startDate)
        {
            return RhproEmployeeLicenseSet
                .Where(s => s.Elfechadesde >= startDate)
                .ToList();
        }
    }
}
