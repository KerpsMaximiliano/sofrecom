using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeProfileHistoryRepository : BaseRepository<EmployeeProfileHistory>, IEmployeeProfileHistoryRepository
    {
        private readonly DbSet<EmployeeProfileHistory> dbSet;

        public EmployeeProfileHistoryRepository(SofcoContext context) : base(context)
        {
            dbSet = context.Set<EmployeeProfileHistory>();
        }

        public void Save(EmployeeProfileHistory employeeProfileHistory)
        {
            Insert(employeeProfileHistory);

            context.SaveChanges();
        }

        public List<EmployeeProfileHistory> GetByEmployeeNumber(string employeeNumber)
        {
            return dbSet.Where(s => s.EmployeeNumber == employeeNumber).ToList();
        }
    }
}
