using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Managers;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;

namespace Sofco.Framework.Managers
{
    public class PurchaseOrderApprovalDelegateManager : IPurchaseOrderApprovalDelegateManager
    {
        private readonly IUserDelegateRepository userDelegateRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IAreaData areaData;

        private readonly ISectorData sectorData;

        private readonly IUserData userData;

        private readonly ISessionManager sessionManager;

        private readonly AppSetting appSetting;

        private List<UserDelegateType> types;

        public PurchaseOrderApprovalDelegateManager(IOptions<AppSetting> appSetting, ISessionManager sessionManager, IUnitOfWork unitOfWork, IUserDelegateRepository userDelegateRepository, IAreaData areaData, ISectorData sectorData, IUserData userData)
        {
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
            this.userDelegateRepository = userDelegateRepository;
            this.areaData = areaData;
            this.sectorData = sectorData;
            this.userData = userData;
            this.appSetting = appSetting.Value;
            SetTypes();
        }

        public List<Role> GetDelegatedRoles()
        {
            var roles = new List<Role>();

            roles.AddRange(GetApprovalComplicance());

            roles.AddRange(GetApprovalCommercial());

            roles.AddRange(GetApprovalOperation());

            roles.AddRange(GetApprovalDaf());

            if (roles.Any())
            {
                roles.Add(unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderApprovalCode));
            }

            roles.AddRange(GetAdditionalDelegateRoles());

            return roles;
        }

        private void SetTypes()
        {
            types = new List<UserDelegateType>
            {
                UserDelegateType.PurchaseOrderApprovalCommercial,
                UserDelegateType.PurchaseOrderApprovalCompliance,
                UserDelegateType.PurchaseOrderApprovalDaf,
                UserDelegateType.PurchaseOrderApprovalOperation
            };
        }

        private List<Role> GetApprovalComplicance()
        {
            var userName = sessionManager.GetUserEmail();

            var isValid = unitOfWork.UserRepository.HasComplianceGroup(userName);

            return !isValid
                ? new List<Role>()
                : new List<Role> {unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderComplianceApprovalCode)};
        }

        private List<Role> GetApprovalCommercial()
        {
            var isValid = areaData.GetAll().Any(s => s.ResponsableUser.Email == sessionManager.GetUserEmail());

            return !isValid
                ? new List<Role>()
                : new List<Role> {unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderCommercialApprovalCode)};
        }

        private List<Role> GetApprovalOperation()
        {
            var isValid = sectorData.GetAll().Any(s => s.ResponsableUser.Email == sessionManager.GetUserEmail());

            return !isValid
                ? new List<Role>()
                : new List<Role> {unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderOperationApprovalCode)};
        }

        private List<Role> GetApprovalDaf()
        {
            var isValid = unitOfWork.UserRepository.HasDafPurchaseOrderGroup(sessionManager.GetUserEmail());

            return !isValid
                ? new List<Role>()
                : new List<Role> {unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderOperationApprovalCode)};
        }

        private List<Role> GetAdditionalDelegateRoles()
        {
            var result = new List<Role>();

            var currentUser = userData.GetCurrentUser();

            var delegates = userDelegateRepository.GetByUserId(currentUser.Id, types);
            foreach (var userDelegate in delegates)
            {
                var roleCode = string.Empty;

                switch (userDelegate.Type)
                {
                    case UserDelegateType.PurchaseOrderApprovalCompliance:
                        roleCode = appSetting.PurchaseOrderComplianceApprovalCode;
                        break;
                    case UserDelegateType.PurchaseOrderApprovalCommercial:
                        roleCode = appSetting.PurchaseOrderCommercialApprovalCode;
                        break;
                    case UserDelegateType.PurchaseOrderApprovalOperation:
                        roleCode = appSetting.PurchaseOrderOperationApprovalCode;
                        break;
                    case UserDelegateType.PurchaseOrderApprovalDaf:
                        roleCode = appSetting.PurchaseOrderDafApprovalCode;
                        break;
                }

                if (string.IsNullOrEmpty(roleCode)) continue;

                result.Add(unitOfWork.RoleRepository.GetByCode(roleCode));
            }

            return result;
       }
    }
}
