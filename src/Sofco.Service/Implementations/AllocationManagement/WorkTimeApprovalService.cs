using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalService : IWorkTimeApprovalService
    {
        private readonly IWorkTimeApprovalRepository workTimeApprovalRepository;

        public WorkTimeApprovalService(IWorkTimeApprovalRepository workTimeApprovalRepository)
        {
            this.workTimeApprovalRepository = workTimeApprovalRepository;
        }


        public Response<List<WorkTimeApproval>> GetAll()
        {
            return new Response<List<WorkTimeApproval>>
            {
                Data = workTimeApprovalRepository.GetAll()
            };
        }

        public Response<List<WorkTimeApproval>> Save(List<WorkTimeApproval> workTimeApprovals)
        {
            workTimeApprovalRepository.Save(workTimeApprovals);

            return new Response<List<WorkTimeApproval>>
            {
                Data = workTimeApprovals
            };
        }

        public Response Delete(int workTimeApprovalId)
        {
            workTimeApprovalRepository.Delete(workTimeApprovalId);

            return new Response();
        }
    }
}
