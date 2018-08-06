using System;
using System.Linq;
using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeLicenseRepository : BaseRepository<EmployeeLicense>, IEmployeeLicenseRepository
    {
        public EmployeeLicenseRepository(SofcoContext context) : base(context)
        {
        }

        public void Delete(List<EmployeeLicense> employeeLicenses, DateTime startDate)
        {
            var deleteItems = context.EmployeeLicenses.Where(s => s.StartDate >= startDate).ToList();

            Delete(deleteItems);
        }

        public void Save(List<EmployeeLicense> employeeLicense)
        {
            foreach (var item in employeeLicense)
            {
                Insert(item);
            }

            context.SaveChanges();
        }
    }
}
