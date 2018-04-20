using System;
using System.Collections.Generic;
using Sofco.Common.Security;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalService : IWorkTimeApprovalService
    {
        private readonly IWorkTimeApprovalRepository workTimeApprovalRepository;

        private readonly IUserData userData;

        private readonly IEmployeeRepository employeeRepository;

        public WorkTimeApprovalService(IWorkTimeApprovalRepository workTimeApprovalRepository, IUserData userData, IEmployeeRepository employeeRepository)
        {
            this.workTimeApprovalRepository = workTimeApprovalRepository;
            this.userData = userData;
            this.employeeRepository = employeeRepository;
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
            var response = ValidateSave(workTimeApprovals);

            if (response.HasErrors())
                return response;

            ResolveUserId(workTimeApprovals);

            workTimeApprovalRepository.Save(workTimeApprovals);

            response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApproverAdded);
            response.Data = workTimeApprovals;

            return response;
        }

        public Response Delete(int workTimeApprovalId)
        {
            workTimeApprovalRepository.Delete(workTimeApprovalId);

            var response = new Response();
            response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApproverDeleted);

            return response;
        }

        private Response<List<WorkTimeApproval>> ValidateSave(List<WorkTimeApproval> workTimeApprovals)
        {
            var response = new Response<List<WorkTimeApproval>>();

            if (workTimeApprovals == null)
            {
                response.AddError(Resources.Common.ErrorSave);

                return response;
            }

            foreach (var workTimeApproval in workTimeApprovals)
            {
                if (workTimeApproval.ApprovalUserId == 0
                    || workTimeApproval.ServiceId == Guid.Empty
                    || workTimeApproval.EmployeeId == 0)
                {
                    response.AddError(Resources.Common.ErrorSave);
                }
            }

            return response;
        }

        private void ResolveUserId(List<WorkTimeApproval> workTimeApprovals)
        {
            foreach (var workTimeApproval in workTimeApprovals)
            {
                var email = employeeRepository.GetById(workTimeApproval.EmployeeId).Email;

                if(string.IsNullOrEmpty(email)) continue;

                var userName = email.Split('@')[0];

                var user = userData.GetByUserName(userName);

                if (user != null)
                {
                    workTimeApproval.UserId = user.Id;
                }
            }
        }
    }
}
