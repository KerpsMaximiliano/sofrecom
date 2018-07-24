using System;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeProfileHistoryRepository : BaseRepository<EmployeeProfileHistory>, IEmployeeProfileHistoryRepository
    {
        public EmployeeProfileHistoryRepository(SofcoContext context) : base(context)
        {
        }

        public void Save(EmployeeProfileHistory employeeProfileHistory)
        {
            Insert(employeeProfileHistory);

            context.SaveChanges();
        }
    }
}
