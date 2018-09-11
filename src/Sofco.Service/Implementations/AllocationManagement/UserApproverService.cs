using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class UserApproverService : IUserApproverService
    {
        private readonly IUserApproverRepository userApproverRepository;

        private readonly IUserData userData;

        private readonly IEmployeeRepository employeeRepository;

        private readonly IMapper mapper;

        private readonly ILogMailer<UserApproverService> logger;

        public UserApproverService(IUserApproverRepository userApproverRepository, 
            IUserData userData, 
            IEmployeeRepository employeeRepository,
            ILogMailer<UserApproverService> logger,
            IMapper mapper)
        {
            this.userApproverRepository = userApproverRepository;
            this.userData = userData;
            this.employeeRepository = employeeRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Response<List<UserApproverModel>> Save(List<UserApproverModel> userApprovers, UserApproverType type)
        {
            var response = ValidateSave(userApprovers);

            if (response.HasErrors())
                return response;

            try
            {
                ResolveUserId(userApprovers);

                userApproverRepository.Save(Translate(userApprovers, type));

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

        public Response<List<UserSelectListItem>> GetApprovers(UserApproverQuery query, UserApproverType type)
        {
            if (query.AnalyticId == 0)
            {
                return new Response<List<UserSelectListItem>> { Data = new List<UserSelectListItem>() };
            }

            var workTimeApprovals = userApproverRepository.GetByAnalyticId(query.AnalyticId, type);

            var userIds = workTimeApprovals.Select(s => s.ApproverUserId).Distinct().ToList();

            var users = userIds.Select(userId => userData.GetUserLiteById(userId))
                .Select(userLite => new UserSelectListItem
                {
                    Id = userLite.Id.ToString(),
                    Text = userLite.Name
                })
                .ToList();

            return new Response<List<UserSelectListItem>>
            {
                Data = users
            };
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

        private List<UserApprover> Translate(List<UserApproverModel> models, UserApproverType type)
        {
            var result = mapper.Map<List<UserApproverModel>, List<UserApprover>>(models);

            result.ForEach(x => x.Type = type);

            return result;
        }
    }
}
