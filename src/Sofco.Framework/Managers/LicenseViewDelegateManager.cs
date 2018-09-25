using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;

namespace Sofco.Framework.Managers
{
    public class LicenseViewDelegateManager : ILicenseViewDelegateManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly AppSetting appSetting;

        public LicenseViewDelegateManager(IUnitOfWork unitOfWork, IUserData userData, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            appSetting = appSettingOptions.Value;
        }

        public List<Role> GetDelegatedRoles()
        {
            var roles = new List<Role>();

            roles.AddRange(GetBaseDelegateRoles());

            if (roles.Any()) return roles;

            roles.AddRange(GetViewRoles());

            return roles;
        }

        private List<Role> GetBaseDelegateRoles()
        {
            var user = userData.GetCurrentUser();

            var isValid = unitOfWork.AnalyticRepository.ExistManagerId(user.Id);

            return !isValid
                ? new List<Role>()
                : new List<Role>
                {
                    unitOfWork.RoleRepository.GetByCode(appSetting.LicenseViewDelegateCode),
                    unitOfWork.RoleRepository.GetByCode(appSetting.LicenseViewCode)
                };
        }

        private List<Role> GetViewRoles()
        {
            var isValid = unitOfWork.UserApproverRepository.HasUserAuthorizer(userData.GetCurrentUser().Id, UserApproverType.LicenseAuthorizer);

            return !isValid
                ? new List<Role>()
                : new List<Role> { unitOfWork.RoleRepository.GetByCode(appSetting.LicenseViewCode) };
        }
    }
}
