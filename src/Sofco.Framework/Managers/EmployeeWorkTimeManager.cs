using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Managers;
using Sofco.Core.Models.AllocationManagement;

namespace Sofco.Framework.Managers
{
    public class EmployeeWorkTimeManager : IEmployeeWorkTimeManager
    {
        private readonly IEmployeeWorkTimeApprovalRepository repository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly ISessionManager sessionManager;

        public EmployeeWorkTimeManager(ISessionManager sessionManager, IEmployeeWorkTimeApprovalRepository repository, IUnitOfWork unitOfWork, IUserData userData)
        {
            this.sessionManager = sessionManager;
            this.repository = repository;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public List<EmployeeWorkTimeApproval> GetByCurrentServices(WorkTimeApprovalQuery query)
        {
            var userMail = sessionManager.GetUserEmail();

            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(userMail);

            List<EmployeeWorkTimeApproval> result;

            if (isDirector)
            {
                result = ResolveApprovalUser(repository.Get(query));

                return ResolveManagers(result);
            }

            var userName = sessionManager.GetUserName();

            var isManager = unitOfWork.UserRepository.HasManagerGroup(userName);

            if (!isManager) return new List<EmployeeWorkTimeApproval>();

            var currentUser = userData.GetByUserName(userName);

            var currentAnalyticIds = unitOfWork.AnalyticRepository.GetByManagerId(currentUser.Id).Select(x => x.Id).ToList();

            result = ResolveApprovalUser(repository.GetByAnalytics(currentAnalyticIds, query));

            return ResolveManagers(result);
        }

        private List<EmployeeWorkTimeApproval> ResolveManagers(List<EmployeeWorkTimeApproval> data)
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

        private List<EmployeeWorkTimeApproval> ResolveApprovalUser(List<EmployeeWorkTimeApproval> data)
        {
            var userIds = data.Where(s => s.WorkTimeApproval != null).Select(s => s.WorkTimeApproval.ApprovalUserId).Distinct().ToList();

            var userNames = userIds.Select(x => userData.GetUserLiteById(x)).ToList();

            foreach (var item in data.Where(s => s.WorkTimeApproval != null))
            {
                var approvalUser = userNames.FirstOrDefault(s => s.Id == item.WorkTimeApproval.ApprovalUserId);
                if (approvalUser == null) continue;
                item.ApprovalName = approvalUser.Name;
            }

            return data;
        }
    }
}
