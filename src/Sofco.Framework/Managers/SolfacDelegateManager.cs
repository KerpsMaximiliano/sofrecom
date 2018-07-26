using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;

namespace Sofco.Framework.Managers
{
    public class SolfacDelegateManager : ISolfacDelegateManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ISessionManager sessionManager;

        private readonly AppSetting appSetting;

        public SolfacDelegateManager(IUnitOfWork unitOfWork, ISessionManager sessionManager, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            appSetting = appSettingOptions.Value;
        }

        public List<Role> GetDelegatedRoles()
        {
            var roles = new List<Role>();

            if (!unitOfWork.UserDelegateRepository.HasUserDelegate(sessionManager.GetUserName(),
                UserDelegateType.Solfac)) return roles;

            roles.Add(unitOfWork.RoleRepository.GetByCode(appSetting.SolfacGeneratorCode));

            return roles;
        }
    }
}
