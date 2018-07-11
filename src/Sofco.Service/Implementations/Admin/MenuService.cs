using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Admin
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserService userService;

        private readonly ISessionManager sessionManager;

        private readonly IPurchaseOrderDelegateManager purchaseOrderDelegateManager;

        private readonly EmailConfig emailConfig;

        private readonly AppSetting appSetting;

        private readonly IUserDelegateRepository userDelegateRepository;

        public MenuService(IUnitOfWork unitOfWork, ISessionManager sessionManager, IUserService userService, IOptions<EmailConfig> emailConfig, IUserDelegateRepository userDelegateRepository, IOptions<AppSetting> appSetting, IPurchaseOrderDelegateManager purchaseOrderDelegateManager)
        {
            this.unitOfWork = unitOfWork;
            this.sessionManager = sessionManager;
            this.userService = userService;
            this.userDelegateRepository = userDelegateRepository;
            this.purchaseOrderDelegateManager = purchaseOrderDelegateManager;
            this.emailConfig = emailConfig.Value;
            this.appSetting = appSetting.Value;
        }

        public Response<MenuResponseModel> GetFunctionalitiesByUserName()
        {
            var roles = GetRoles();

            var modules = unitOfWork.MenuRepository.GetFunctionalitiesByRoles(roles.Select(x => x.Id));

            var model = new MenuResponseModel();

            foreach (var item in modules)
            {
                if (!item.Functionality.Active || !item.Functionality.Module.Active) continue;

                if(model.Menus.Any(x => x.Module == item.Functionality.Module.Code && x.Functionality == item.Functionality.Code)) continue;

                var menu = new MenuModel
                {
                    Description = item.Functionality.Description,
                    Functionality = item.Functionality.Code,
                    Module = item.Functionality.Module.Code
                };

                model.Menus.Add(menu);
            }

            model.IsDirector = userService.HasDirectorGroup();
            model.IsManager = userService.HasManagerGroup();
            model.IsDaf = userService.HasDafGroup();
            model.IsCdg = userService.HasCdgGroup();
            model.IsRrhh = userService.HasRrhhGroup();
            model.DafMail = GetGroupMail(emailConfig.DafCode);
            model.CdgMail = GetGroupMail(emailConfig.CdgCode);
            model.PmoMail = GetGroupMail(emailConfig.PmoCode);
            model.RrhhMail = GetGroupMail(emailConfig.RrhhCode);
            model.SellerMail = GetGroupMail(emailConfig.SellerCode);

            return new Response<MenuResponseModel> {Data = model};
        }

        public string GetGroupMail(string code)
        {
            return unitOfWork.GroupRepository.GetEmail(code);
        }

        private List<Role> GetRoles()
        {
            var userName = sessionManager.GetUserName();

            var groupsId = unitOfWork.UserGroupRepository.GetGroupsId(userName);

            var roles = unitOfWork.RoleRepository.GetRolesByGroup(groupsId).ToList();

            roles.AddRange(GetPurchaseOrderRoles());

            if (userDelegateRepository.HasUserDelegate(userName, UserDelegateType.Solfac))
            {
                var solfacDelegateRole = unitOfWork.RoleRepository.GetByCode(appSetting.SolfacGeneratorCode);

                roles.Add(solfacDelegateRole);
            }

            return roles;
        }

        private List<Role> GetPurchaseOrderRoles()
        {
            return purchaseOrderDelegateManager.GetPurchaseOrderRoles();
        }
    }
}
