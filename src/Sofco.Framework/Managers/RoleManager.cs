using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Domain.Models.Admin;

namespace Sofco.Framework.Managers
{
    public class RoleManager : IRoleManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ISessionManager sessionManager;

        private readonly IPurchaseOrderApprovalDelegateManager purchaseOrderApprovalDelegateManager;

        private readonly IPurchaseOrderActiveDelegateManager purchaseOrderActiveDelegateManager;

        private readonly ILicenseViewDelegateManager licenseViewDelegateManager;

        private readonly ISolfacDelegateManager solfacDelegateManager;

        private readonly IWorkTimeApproverDelegateManager workTimeApproverDelegateManager;

        private readonly IUserData userData;

        public RoleManager(IPurchaseOrderApprovalDelegateManager purchaseOrderApprovalDelegateManager, IPurchaseOrderActiveDelegateManager purchaseOrderActiveDelegateManager, ILicenseViewDelegateManager licenseViewDelegateManager, ISolfacDelegateManager solfacDelegateManager, IUnitOfWork unitOfWork, ISessionManager sessionManager, IWorkTimeApproverDelegateManager workTimeApproverDelegateManager, IUserData userData)
        {
            this.purchaseOrderApprovalDelegateManager = purchaseOrderApprovalDelegateManager;
            this.purchaseOrderActiveDelegateManager = purchaseOrderActiveDelegateManager;
            this.licenseViewDelegateManager = licenseViewDelegateManager;
            this.solfacDelegateManager = solfacDelegateManager;
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.workTimeApproverDelegateManager = workTimeApproverDelegateManager;
            this.userData = userData;
        }

        public List<Role> GetRoles()
        {
            var groupsId = unitOfWork.UserGroupRepository.GetGroupsId(sessionManager.GetUserName());

            var roles = unitOfWork.RoleRepository.GetRolesByGroup(groupsId).ToList();

            roles.AddRange(purchaseOrderApprovalDelegateManager.GetDelegatedRoles());

            roles.AddRange(purchaseOrderActiveDelegateManager.GetDelegatedRoles());

            roles.AddRange(licenseViewDelegateManager.GetDelegatedRoles());

            roles.AddRange(solfacDelegateManager.GetDelegatedRoles());

            roles.AddRange(workTimeApproverDelegateManager.GetDelegatedRoles());

            return roles;
        }

        public bool HasFullAccess()
        {
            var currentUserEmail = userData.GetCurrentUser().Email;

            var hasDirectorGroup = unitOfWork.UserRepository.HasDirectorGroup(currentUserEmail);
            var hasCommercialGroup = unitOfWork.UserRepository.HasComercialGroup(currentUserEmail);
            var hasCdgGroup = unitOfWork.UserRepository.HasCdgGroup(currentUserEmail);
            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUserEmail);

            return hasDirectorGroup || hasCommercialGroup || hasCdgGroup || hasDafGroup;
        }
    }
}
