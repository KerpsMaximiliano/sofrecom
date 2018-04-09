using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IWorkTimeApprovalRepository
    {
        List<WorkTimeApproval> GetAll();

        void Save(List<WorkTimeApproval> workTimeApprovals);

        void Delete(int workTimeApprovalId);
    }
}