using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Framework.Managers.UserApprovers
{
    public class WorkTimeApproverEmployeeManager : IWorkTimeApproverEmployeeManager
    {
        private readonly IUserApproverEmployeeRepository repository;

        private readonly IUserApproverManager approverManager;

        private readonly UserApproverType Type = UserApproverType.WorkTime;

        public WorkTimeApproverEmployeeManager(IUserApproverEmployeeRepository repository, IUserApproverManager approverManager)
        {
            this.repository = repository;
            this.approverManager = approverManager;
        }

        public List<UserApproverEmployee> Get(UserApproverQuery query)
        {
            if (approverManager.IsDirector()) return GetAll(query);

            if (!approverManager.IsManager()) return new List<UserApproverEmployee>();

            return approverManager.GetByCurrentManager(query, Type);
        }

        private List<UserApproverEmployee> GetAll(UserApproverQuery query)
        {
            return approverManager.ResolveData(repository.GetByAllocations(query, Type));
        }
    }
}
