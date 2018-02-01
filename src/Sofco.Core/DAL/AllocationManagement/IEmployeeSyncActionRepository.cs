using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeSyncActionRepository : IBaseRepository<EmployeeSyncAction>
    {
        void Save(List<EmployeeSyncAction> employeeSyncActions);

        bool Exist(int id);
    }
}