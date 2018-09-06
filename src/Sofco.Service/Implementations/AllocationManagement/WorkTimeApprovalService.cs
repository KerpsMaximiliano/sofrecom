using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalService : IWorkTimeApprovalService
    {
        private readonly IUserApproverRepository userApproverRepository;

        private readonly IUserData userData;

        private readonly IEmployeeRepository employeeRepository;

        private readonly IMapper mapper;

        private readonly ILogMailer<WorkTimeApprovalService> logger;

        public WorkTimeApprovalService(IUserApproverRepository userApproverRepository, 
            IUserData userData, 
            IEmployeeRepository employeeRepository,
            ILogMailer<WorkTimeApprovalService> logger,
            IMapper mapper)
        {
            this.userApproverRepository = userApproverRepository;
            this.userData = userData;
            this.employeeRepository = employeeRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Response<List<UserApproverModel>> Save(List<UserApproverModel> userApprovers)
        {
            var response = ValidateSave(userApprovers);

            if (response.HasErrors())
                return response;

            try
            {
                ResolveUserId(userApprovers);

                userApproverRepository.Save(Translate(userApprovers));

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApproverAdded);
                response.Data = userApprovers;
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
                userApproverRepository.Delete(workTimeApprovalId);

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApproverDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private Response<List<UserApproverModel>> ValidateSave(List<UserApproverModel> userApprovers)
        {
            var response = new Response<List<UserApproverModel>>();

            if (userApprovers == null)
            {
                response.AddError(Resources.Common.ErrorSave);

                return response;
            }

            foreach (var workTimeApproval in userApprovers)
            {
                if (workTimeApproval.ApproverUserId == 0
                    || workTimeApproval.AnalyticId == 0
                    || workTimeApproval.EmployeeId == 0)
                {
                    response.AddError(Resources.Common.ErrorSave);
                }
            }

            return response;
        }

        private void ResolveUserId(List<UserApproverModel> userApprovers)
        {
            foreach (var workTimeApproval in userApprovers)
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

        private List<UserApprover> Translate(List<UserApproverModel> models)
        {
            return mapper.Map<List<UserApproverModel>, List<UserApprover>>(models);
        }
    }
}
