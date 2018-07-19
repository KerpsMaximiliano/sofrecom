using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;

namespace Sofco.Framework.Managers
{
    public class PurchaseOrderActiveDelegateManager : IPurchaseOrderActiveDelegateManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly ISessionManager sessionManager;

        private readonly AppSetting appSetting;

        public PurchaseOrderActiveDelegateManager(IOptions<AppSetting> appSetting, ISessionManager sessionManager, IUnitOfWork unitOfWork, IUserData userData)
        {
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.appSetting = appSetting.Value;
        }

        public List<Role> GetDelegatedRoles()
        {
            var roles = new List<Role>();

            roles.AddRange(GetActiveDelegateRoles());

            if (roles.Any()) return roles;

            roles.AddRange(GetActiveViewRoles());

            return roles;
        }

        private List<Role> GetActiveViewRoles()
        {
            var isValid = unitOfWork.UserDelegateRepository.HasUserDelegate(sessionManager.GetUserName(), UserDelegateType.PurchaseOrderActive);

            var userMail = sessionManager.GetUserEmail();
            if (!isValid)
            {
                isValid = unitOfWork.UserRepository.HasDirectorGroup(userMail);
            }
            if (!isValid)
            {
                isValid = unitOfWork.UserRepository.HasDafGroup(userMail);
            }
            if (!isValid)
            {
                isValid = unitOfWork.UserRepository.HasCdgGroup(userMail);
            }
            if (!isValid)
            {
                isValid = unitOfWork.UserRepository.HasComplianceGroup(userMail);
            }
            if (!isValid)
            {
                isValid = unitOfWork.UserRepository.HasComercialGroup(userMail);
            }

            return !isValid
                ? new List<Role>()
                : new List<Role>{unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderActiveViewCode)};
        }

        private List<Role> GetActiveDelegateRoles()
        {
            var user = userData.GetCurrentUser();

            var isValid = unitOfWork.AnalyticRepository.ExistManagerId(user.Id);

            return !isValid
                ? new List<Role>()
                : new List<Role>
                {
                    unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderActiveDelegateCode),
                    unitOfWork.RoleRepository.GetByCode(appSetting.PurchaseOrderActiveViewCode)
                };
        }
    }
}
