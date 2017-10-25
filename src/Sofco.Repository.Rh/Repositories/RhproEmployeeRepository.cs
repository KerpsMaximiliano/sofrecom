using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sofco.Repository.Rh.Repositories.Interfaces;
using Sofco.Domain.Rh.Rhpro;

namespace Sofco.Repository.Rh.Repositories
{
    public class RhproEmployeeRepository : IRhproEmployeeRepository
    {
        protected readonly RhproContext context;

        private DbSet<RhproEmployee> rhproEmployeeSet { get; set; }

        private DbSet<RhproEmployeeLicense> rhproEmployeeLicenseSet { get; set; }

        private DbSet<RhproLicenseType> rhproLicenseTypeSet { get; set; }

        public RhproEmployeeRepository(RhproContext context)
        {
            rhproEmployeeSet = context.Set<RhproEmployee>();

            rhproEmployeeLicenseSet = context.Set<RhproEmployeeLicense>();

            rhproLicenseTypeSet = context.Set<RhproLicenseType>();
        }

        public IList<RhproEmployee> GetAll()
        {
            return rhproEmployeeSet.ToList();
        }

        public IList<RhproEmployeeLicense> GetLicenses()
        {
            return rhproEmployeeLicenseSet.ToList();
        }

        public IList<RhproLicenseType> GetLicenseTypes()
        {
            return rhproLicenseTypeSet.ToList();
        }
    }
}
