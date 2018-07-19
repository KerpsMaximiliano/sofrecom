using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Framework.Managers
{
    public class EmployeeWorkTimeManager : IEmployeeWorkTimeManager
    {
        private readonly IEmployeeWorkTimeApprovalRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IAnalyticData analyticData;

        private readonly ISessionManager sessionManager;

        public EmployeeWorkTimeManager(ISessionManager sessionManager, IEmployeeWorkTimeApprovalRepository repository, IUnitOfWork unitOfWork, IUserData userData, IAnalyticData analyticData)
        {
            this.sessionManager = sessionManager;
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.analyticData = analyticData;
        }

        public List<WorkTimeApprovalEmployee> GetByCurrentServices(WorkTimeApprovalQuery query)
        {
            var userMail = sessionManager.GetUserEmail();

            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(userMail);

            List<WorkTimeApprovalEmployee> result;

            if (isDirector)
            {
                result = ResolveApprovalUser(repository.Get(query));

                return ResolveManagers(result);
            }

            var userName = sessionManager.GetUserName();

            var isManager = unitOfWork.UserRepository.HasManagerGroup(userName);

            if (!isManager) return new List<WorkTimeApprovalEmployee>();

            List<WorkTimeApprovalEmployee> analytics;

            if (query.AnalyticId > 0)
            {
                analytics = repository.Get(query);
            }
            else
            {
                var currentUser = userData.GetCurrentUser();

                var currentAnalyticIds = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id).Select(x => x.Id)
                    .ToList();

                analytics = repository.GetByAnalytics(currentAnalyticIds, query.ApprovalId);
            }

            result = ResolveApprovalUser(analytics);

            return ResolveManagers(result);
        }

        private List<WorkTimeApprovalEmployee> ResolveManagers(List<WorkTimeApprovalEmployee> data)
        {
            var managerIds = data.Where(s => s.ManagerId.HasValue).Select(s => s.ManagerId.Value).Distinct().ToList();

            var managers = managerIds.Select(managerId => userData.GetUserLiteById(managerId)).ToList();

            foreach (var item in data)
            {
                var manager = managers.FirstOrDefault(s => s.Id == item.ManagerId);
                if(manager == null) continue;
                item.Manager = manager.Name;
            }

            return data;
        }

        private List<WorkTimeApprovalEmployee> ResolveApprovalUser(List<WorkTimeApprovalEmployee> data)
        {
            var userIds = data.Where(s => s.WorkTimeApproval != null).Select(s => s.WorkTimeApproval.ApprovalUserId).Distinct().ToList();

            var userNames = userIds.Select(x => userData.GetUserLiteById(x)).ToList();

            foreach (var item in data.Where(s => s.WorkTimeApproval != null))
            {
                var approvalUser = userNames.FirstOrDefault(s => s.Id == item.WorkTimeApproval.ApprovalUserId);
                if (approvalUser == null) continue;
                item.ApprovalName = approvalUser.Name;
            }

            data = ResolveAnalytics(data);

            return data;
        }

        private List<WorkTimeApprovalEmployee> ResolveAnalytics(List<WorkTimeApprovalEmployee> data)
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
    }
}
