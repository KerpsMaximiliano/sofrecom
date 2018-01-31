using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeHistoryRepository : BaseRepository<EmployeeHistory>, IEmployeeHistoryRepository
    {
        public EmployeeHistoryRepository(SofcoContext context) : base(context)
        {
        }

        public void Save(EmployeeHistory employeeHistory)
        {
            Insert(employeeHistory);

            context.SaveChanges();
        }
    }
}
