using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
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

        public RoleManager(IPurchaseOrderApprovalDelegateManager purchaseOrderApprovalDelegateManager, IPurchaseOrderActiveDelegateManager purchaseOrderActiveDelegateManager, ILicenseViewDelegateManager licenseViewDelegateManager, ISolfacDelegateManager solfacDelegateManager, IUnitOfWork unitOfWork, ISessionManager sessionManager, IWorkTimeApproverDelegateManager workTimeApproverDelegateManager)
        {
            this.purchaseOrderApprovalDelegateManager = purchaseOrderApprovalDelegateManager;
            this.purchaseOrderActiveDelegateManager = purchaseOrderActiveDelegateManager;
            this.licenseViewDelegateManager = licenseViewDelegateManager;
            this.solfacDelegateManager = solfacDelegateManager;
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.workTimeApproverDelegateManager = workTimeApproverDelegateManager;
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
    }
}
