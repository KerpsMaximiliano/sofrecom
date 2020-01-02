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

            var employeeIds = unitOfWork.DelegationRepository
                .GetByGrantedUserIdAndType(approverUserId, DelegationType.LicenseAuthorizer)
                .Select(x => x.EmployeeSourceId.GetValueOrDefault()).ToList();

            var result = unitOfWork.LicenseRepository.GetByEmployee(employeeIds);

            return result.ToList();
        }

        public List<string> GetEmailApproversByEmployeeId(int employeeId)
        {
            var delegations = unitOfWork.DelegationRepository.GetByEmployeeSourceId(employeeId, DelegationType.LicenseAuthorizer);

            return delegations.Select(x => x.GrantedUser.Email).ToList();
        }

        public List<License> GetByCurrentByStatus(LicenseStatus statusId)
        {
            int approverUserId = userData.GetCurrentUser().Id;

            var employeeIds = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(approverUserId,DelegationType.LicenseAuthorizer).Select(x => x.EmployeeSourceId.GetValueOrDefault()).ToList();

            var result = unitOfWork.LicenseRepository.GetByEmployeeAndStatus(employeeIds, statusId);

            return result.ToList();
        }

        public List<LicenseListModel> ResolveApprovers(List<LicenseListModel> models)
        {
            var employeeIds = models.Select(s => s.EmployeeId).Distinct().ToList();

            var delegations = unitOfWork.DelegationRepository.GetByEmployeeSourceId(employeeIds, DelegationType.LicenseAuthorizer);

            foreach (var item in models)
            {
                var delegation = delegations.FirstOrDefault(s => s.EmployeeSourceId == item.EmployeeId);

                item.AuthorizerName = delegation?.GrantedUser.Name;
            }

            return models;
        }

        public bool HasUserAuthorizer()
        {
            var userId = userData.GetCurrentUser().Id;

            return unitOfWork.DelegationRepository.ExistByGrantedUserIdAndType(userId, DelegationType.LicenseAuthorizer);
        }
    }
}
