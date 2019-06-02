using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;

namespace Sofco.Framework.Managers
{
    public class WorkTimeApproverDelegateManager : IWorkTimeApproverDelegateManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly AppSetting appSetting;

        public WorkTimeApproverDelegateManager(IUnitOfWork unitOfWork, IUserData userData, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            appSetting = appSettingOptions.Value;
        }

        public List<Role> GetDelegatedRoles()
        {
            var isValid = unitOfWork.UserApproverRepository.HasUserAuthorizer(
                userData.GetCurrentUser().Id, 
                UserApproverType.WorkTime);

            return isValid
                ? new List<Role> { unitOfWork.RoleRepository.GetByCode(appSetting.WorkTimeApprovalCode), unitOfWork.RoleRepository.GetByCode(appSetting.WorkTimeMassiveImportCode) }
                : new List<Role>();
        }
    }
}
