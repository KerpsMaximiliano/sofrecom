using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Framework.Managers.UserApprovers
{
    public class UserApproverManager : IUserApproverManager
    {
        private readonly IUserApproverEmployeeRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IAnalyticData analyticData;

        private readonly ISessionManager sessionManager;

        public UserApproverManager(IUserData userData, IAnalyticData analyticData, IUserApproverEmployeeRepository repository, IUnitOfWork unitOfWork, ISessionManager sessionManager)
        {
            this.userData = userData;
            this.analyticData = analyticData;
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
        }

        public List<UserApproverEmployee> GetByCurrentManager(UserApproverQuery query, UserApproverType type)
        {
            List<UserApproverEmployee> userApproverEmployee;

            if (query.AnalyticId > 0)
            {
                userApproverEmployee = repository.GetByAllocations(query, type);
            }
            else
            {
                var currentUser = userData.GetCurrentUser();

                var currentAnalyticIds = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id).Select(x => x.Id)
                    .ToList();

                userApproverEmployee = repository.GetByAllocationAnalytics(currentAnalyticIds, query.ApprovalId, type);
            }

            var result = ResolveApprovalUser(userApproverEmployee);

            return ResolveManagers(result);
        }

        public List<UserApproverEmployee> ResolveManagers(List<UserApproverEmployee> data)
        {
            var managerIds = data.Where(s => s.ManagerId.HasValue).Select(s => s.ManagerId.Value).Distinct().ToList();

            var managers = managerIds.Select(managerId => userData.GetUserLiteById(managerId)).ToList();

            foreach (var item in data)
            {
                var manager = managers.FirstOrDefault(s => s.Id == item.ManagerId);
                if (manager == null) continue;
                item.Manager = manager.Name;
            }

            return data;
        }

        public List<UserApproverEmployee> ResolveApprovalUser(List<UserApproverEmployee> data)
        {
            var userIds = data.Where(s => s.UserApprover != null).Select(s => s.UserApprover.ApproverUserId).Distinct().ToList();

            var userNames = userIds.Select(x => userData.GetUserLiteById(x)).ToList();

            foreach (var item in data.Where(s => s.UserApprover != null))
            {
                var approvalUser = userNames.FirstOrDefault(s => s.Id == item.UserApprover.ApproverUserId);
                if (approvalUser == null) continue;
                item.ApprovalName = approvalUser.Name;
            }

            data = ResolveAnalytics(data);

            return data;
        }

        private List<UserApproverEmployee> ResolveAnalytics(List<UserApproverEmployee> data)
        {
            var analyticsIds = data.Where(s => s.AnalyticId != null).Select(s => s.AnalyticId.Value).Distinct().ToList();

            var analytics = analyticsIds.Select(x => analyticData.GetLiteById(x)).ToList();

            foreach (var item in data.Where(s => s.AnalyticId != null))
            {
                var analytic = analytics.FirstOrDefault(s => s.Id == item.AnalyticId);
                if (analytic == null) continue;
                item.Analytic = $"{analytic.Title} - {analytic.Name}";
            }

            return data;
        }

        public List<UserApproverEmployee> ResolveData(List<UserApproverEmployee> data)
        {
            var result = ResolveApprovalUser(data);

            return ResolveManagers(result);
        }

        public bool IsManager()
        {
            var userName = sessionManager.GetUserName();

            return unitOfWork.UserRepository.HasManagerGroup(userName);
        }

        public bool IsDirector()
        {
            var currentUser = userData.GetCurrentUser();

            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(currentUser.Email)
                             || unitOfWork.SectorRepository.HasSector(currentUser.Id);

            return isDirector;
        }
    }
}
