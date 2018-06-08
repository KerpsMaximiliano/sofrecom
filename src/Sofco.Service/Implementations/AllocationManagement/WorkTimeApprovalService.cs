using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Logger;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalService : IWorkTimeApprovalService
    {
        private readonly IWorkTimeApprovalRepository workTimeApprovalRepository;

        private readonly IUserData userData;

        private readonly IEmployeeRepository employeeRepository;

        private readonly IMapper mapper;

        private readonly ILogMailer<WorkTimeApprovalService> logger;

        public WorkTimeApprovalService(IWorkTimeApprovalRepository workTimeApprovalRepository, 
            IUserData userData, 
            IEmployeeRepository employeeRepository,
            ILogMailer<WorkTimeApprovalService> logger,
            IMapper mapper)
        {
            this.workTimeApprovalRepository = workTimeApprovalRepository;
            this.userData = userData;
            this.employeeRepository = employeeRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Response<List<WorkTimeApproval>> GetAll()
        {
            return new Response<List<WorkTimeApproval>>
            {
                Data = workTimeApprovalRepository.GetAll()
            };
        }

        public Response<List<WorkTimeApprovalModel>> Save(List<WorkTimeApprovalModel> workTimeApprovals)
        {
            var response = ValidateSave(workTimeApprovals);

            if (response.HasErrors())
                return response;

            try
            {
                ResolveUserId(workTimeApprovals);

                workTimeApprovalRepository.Save(Translate(workTimeApprovals));

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApproverAdded);
                response.Data = workTimeApprovals;
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Delete(int workTimeApprovalId)
        {
            var response = new Response();

            try
            {
                workTimeApprovalRepository.Delete(workTimeApprovalId);

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApproverDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private Response<List<WorkTimeApprovalModel>> ValidateSave(List<WorkTimeApprovalModel> workTimeApprovals)
        {
            var response = new Response<List<WorkTimeApprovalModel>>();

            if (workTimeApprovals == null)
            {
                response.AddError(Resources.Common.ErrorSave);

                return response;
            }

            foreach (var workTimeApproval in workTimeApprovals)
            {
                if (workTimeApproval.ApprovalUserId == 0
                    || workTimeApproval.AnalyticId == 0
                    || workTimeApproval.EmployeeId == 0)
                {
                    response.AddError(Resources.Common.ErrorSave);
                }
            }

            return response;
        }

        private void ResolveUserId(List<WorkTimeApprovalModel> workTimeApprovals)
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

        private List<WorkTimeApproval> Translate(List<WorkTimeApprovalModel> workTimeApprovalModels)
        {
            return mapper.Map<List<WorkTimeApprovalModel>, List<WorkTimeApproval>>(workTimeApprovalModels);
        }
    }
}
