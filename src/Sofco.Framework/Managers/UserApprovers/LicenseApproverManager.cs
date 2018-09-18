using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.Rrhh;
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
            var approverUserId = userData.GetCurrentUser().Id;

            var employeeIds = unitOfWork.UserApproverRepository
                .GetByApproverUserId(approverUserId, UserApproverType.LicenseAuthorizer)
                .Select(s => s.EmployeeId).ToList();

            var result = unitOfWork.LicenseRepository.GetByEmployee(employeeIds);

            return result.ToList();
        }

        public List<string> GetEmailApproversByUserId(int userId)
        {
            return unitOfWork.UserApproverRepository
                .GetApproverByUserId(userId, UserApproverType.LicenseAuthorizer)
                .Select(s => s.Email).ToList();
        }

        public List<string> GetEmailApproversByEmployeeId(int employeeId)
        {
            return unitOfWork.UserApproverRepository
                .GetApproverByEmployeeId(employeeId, UserApproverType.LicenseAuthorizer)
                .Select(s => s.Email).ToList();
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

        public List<LicenseListModel> ResolveApprovers(List<LicenseListModel> models)
        {
            var employeeIds = models.Select(s => s.EmployeeId).Distinct().ToList();

            var userApprovers = unitOfWork.UserApproverRepository.GetByEmployeeIds(employeeIds, UserApproverType.LicenseAuthorizer);

            foreach (var item in models)
            {
                var userApprover = userApprovers.FirstOrDefault(s => s.EmployeeId == item.EmployeeId);

                item.AuthorizerName = userApprover?.ApproverUser.Name;
            }

            return models;
        }

        public bool HasUserAuthorizer()
        {
            var userId = userData.GetCurrentUser().Id;

            return unitOfWork.UserApproverRepository.HasUserAuthorizer(userId, UserApproverType.LicenseAuthorizer);
        }
    }
}
