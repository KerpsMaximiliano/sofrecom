using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Framework.Managers.UserApprovers
{
    public class LicenseApproverManager : ILicenseApproverManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        public LicenseApproverManager(IUnitOfWork unitOfWork, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public List<License> GetByCurrent()
        {
            int approverUserId = userData.GetCurrentUser().Id;

            var employeeIds = unitOfWork.UserApproverRepository
                .GetByApproverUserId(approverUserId, UserApproverType.LicenseAuthorizer)
                .Select(s => s.EmployeeId).ToList();

            var result = unitOfWork.LicenseRepository.GetByEmployee(employeeIds);

            return result.ToList();
        }

        public List<License> GetByCurrentByStatus(LicenseStatus statusId)
        {
            int approverUserId = userData.GetCurrentUser().Id;

            var employeeIds = unitOfWork.UserApproverRepository
                .GetByApproverUserId(approverUserId, UserApproverType.LicenseAuthorizer)
                .Select(s => s.EmployeeId).ToList();

            var result = unitOfWork.LicenseRepository.GetByEmployeeAndStatus(employeeIds, statusId);

            return result.ToList();
        }
    }
}
