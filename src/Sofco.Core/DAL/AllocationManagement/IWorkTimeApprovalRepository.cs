using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;

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