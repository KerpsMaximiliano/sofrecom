using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Managers;
using Sofco.Core.Models.AllocationManagement;

namespace Sofco.Framework.Managers
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly IEmployeeWorkTimeApprovalRepository repository;

        private readonly ISessionManager sessionManager;

        public EmployeeManager(ISessionManager sessionManager, ICustomerData customerData, IServiceData serviceData, IEmployeeWorkTimeApprovalRepository repository)
        {
            this.sessionManager = sessionManager;
            this.repository = repository;
        }

        public List<EmployeeWorkTimeApproval> GetByCurrentServices()
        {
            //var userMail = sessionManager.GetUserEmail();

            //var isDirector = unitOfWork.UserRepository.HasDirectorGroup(userMail);

            return repository.Get();
        }
    }
}
