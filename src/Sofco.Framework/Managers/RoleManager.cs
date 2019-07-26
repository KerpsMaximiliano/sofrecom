using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
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

        public UserLiteModel CurrentUser { get; set; }

        public RoleManager(IPurchaseOrderApprovalDelegateManager purchaseOrderApprovalDelegateManager, 
            IPurchaseOrderActiveDelegateManager purchaseOrderActiveDelegateManager, 
            ILicenseViewDelegateManager licenseViewDelegateManager, 
            ISolfacDelegateManager solfacDelegateManager, 
            IUnitOfWork unitOfWork, 
            ISessionManager sessionManager,
            IUserData userData,
            IWorkTimeApproverDelegateManager workTimeApproverDelegateManager)
        {
            this.purchaseOrderApprovalDelegateManager = purchaseOrderApprovalDelegateManager;
            this.purchaseOrderActiveDelegateManager = purchaseOrderActiveDelegateManager;
            this.licenseViewDelegateManager = licenseViewDelegateManager;
            this.solfacDelegateManager = solfacDelegateManager;
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.workTimeApproverDelegateManager = workTimeApproverDelegateManager;
            this.userData = userData;

            CurrentUser = userData.GetCurrentUser();
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
            var currentUserEmail = CurrentUser.Email;

            var hasDirectorGroup = unitOfWork.UserRepository.HasDirectorGroup(currentUserEmail);
            var hasCommercialGroup = unitOfWork.UserRepository.HasComercialGroup(currentUserEmail);
            var hasCdgGroup = unitOfWork.UserRepository.HasCdgGroup(currentUserEmail);
            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUserEmail);
            var hasGafGroup = unitOfWork.UserRepository.HasGafGroup(currentUserEmail);
            var hasPmoGroup = unitOfWork.UserRepository.HasPmoGroup(currentUserEmail);
            var hasComplianceGroup = unitOfWork.UserRepository.HasComplianceGroup(currentUserEmail);
            var hasReadOnlyGroup = unitOfWork.UserRepository.HasReadOnlyGroup(currentUserEmail);
            var hasRrhhGroup = unitOfWork.UserRepository.HasRrhhGroup(currentUserEmail);

            return hasDirectorGroup || hasCommercialGroup || hasGafGroup || hasCdgGroup || hasDafGroup || hasPmoGroup || hasReadOnlyGroup || hasRrhhGroup || hasComplianceGroup;
        }

        public bool HasAdvancementAccess()
        {
            var currentUserEmail = CurrentUser.Email;

            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUserEmail);
            var hasGafGroup = unitOfWork.UserRepository.HasGafGroup(currentUserEmail);
            var hasComplianceGroup = unitOfWork.UserRepository.HasComplianceGroup(currentUserEmail);
            var hasReadOnlyGroup = unitOfWork.UserRepository.HasReadOnlyGroup(currentUserEmail);
            var hasRrhhGroup = unitOfWork.UserRepository.HasRrhhGroup(currentUserEmail);

            return  hasGafGroup || hasDafGroup || hasReadOnlyGroup || hasRrhhGroup || hasComplianceGroup;
        }

        public bool HasAccessForRefund()
        {
            var currentUserEmail = CurrentUser.Email;

            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUserEmail);
            var hasGafGroup = unitOfWork.UserRepository.HasGafGroup(currentUserEmail);
            var hasComplianceGroup = unitOfWork.UserRepository.HasComplianceGroup(currentUserEmail);
            var hasReadOnlyGroup = unitOfWork.UserRepository.HasReadOnlyGroup(currentUserEmail);

            return hasGafGroup || hasDafGroup || hasReadOnlyGroup || hasComplianceGroup;
        }

        public bool IsDafOrGaf()
        {
            var currentUserEmail = CurrentUser.Email;

            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUserEmail);
            var hasGafGroup = unitOfWork.UserRepository.HasGafGroup(currentUserEmail);
            var hasReadOnlyGroup = unitOfWork.UserRepository.HasReadOnlyGroup(currentUserEmail);

            return hasGafGroup || hasDafGroup || hasReadOnlyGroup;
        }

        public bool IsDirector()
        {
            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(CurrentUser.Email)
                             || unitOfWork.SectorRepository.HasSector(CurrentUser.Id);

            return isDirector;
        }

        public bool IsCdg()
        {
            var hasCdgGroup = unitOfWork.UserRepository.HasCdgGroup(CurrentUser.Email);

            return hasCdgGroup;
        }

        public bool IsPmo()
        {
            var hasPmoGroup = unitOfWork.UserRepository.HasPmoGroup(CurrentUser.Email);

            return hasPmoGroup;
        }

        public bool IsRrhh()
        {
            var hasRrhhGroup = unitOfWork.UserRepository.HasRrhhGroup(CurrentUser.Email);

            return hasRrhhGroup;
        }

        public bool IsCompliance()
        {
            var hasComplianceGroup = unitOfWork.UserRepository.HasComplianceGroup(CurrentUser.Email);

            return hasComplianceGroup;
        }

        public bool IsManager()
        {
            var isManager = unitOfWork.UserRepository.HasManagerGroup(CurrentUser.UserName);

            return isManager;
        }
    }
}
