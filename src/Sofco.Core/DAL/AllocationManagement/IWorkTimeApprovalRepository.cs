using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IWorkTimeApprovalRepository
    {
        List<WorkTimeApproval> GetAll();

        void Save(List<WorkTimeApproval> workTimeApprovals);

        void Delete(int workTimeApprovalId);

        List<WorkTimeApproval> GetByAnalyticId(int analyticId);

        List<Analytic> GetByAnalyticApproval(int currentUserId);
    }
}