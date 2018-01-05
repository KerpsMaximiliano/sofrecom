using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeSyncActionRepository
    {
        void Save(List<EmployeeSyncAction> employeeSyncActions);
    }
}