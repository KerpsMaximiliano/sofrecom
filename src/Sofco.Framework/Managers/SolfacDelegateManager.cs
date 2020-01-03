using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;

namespace Sofco.Framework.Managers
{
    public class SolfacDelegateManager : ISolfacDelegateManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly AppSetting appSetting;

        public SolfacDelegateManager(IUnitOfWork unitOfWork, IUserData userData, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            appSetting = appSettingOptions.Value;
        }

        public List<Role> GetDelegatedRoles()
        {
            var roles = new List<Role>();

            var currentUser = userData.GetCurrentUser();

            if (!unitOfWork.DelegationRepository.ExistByGrantedUserIdAndType(currentUser.Id, DelegationType.Solfac)) return roles;

            roles.Add(unitOfWork.RoleRepository.GetByCode(appSetting.SolfacGeneratorCode));

            return roles;
        }
    }
}
