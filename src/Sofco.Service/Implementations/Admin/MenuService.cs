using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Data.Billing;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Domain.Utils;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Domain.Enums;

namespace Sofco.Service.Implementations.Admin
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserService userService;

        private readonly IRoleManager roleManager;

        private readonly IAreaData areaData;

        private readonly ISectorData sectorData;

        private readonly EmailConfig emailConfig;

        private readonly IUserData userData;
        
        public MenuService(IUnitOfWork unitOfWork, IUserService userService, IOptions<EmailConfig> emailConfig, IAreaData areaData, ISectorData sectorData, IRoleManager roleManager, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.userService = userService;
            this.areaData = areaData;
            this.sectorData = sectorData;
            this.roleManager = roleManager;
            this.emailConfig = emailConfig.Value;
            this.userData = userData;
        }

        public Response<MenuResponseModel> GetFunctionalitiesByUserName()
        {
            var roles = roleManager.GetRoles();

            var modules = unitOfWork.MenuRepository.GetFunctionalitiesByRoles(roles.Select(x => x.Id));

            var model = new MenuResponseModel();

            foreach (var item in modules)
            {
                if (!item.Functionality.Active || !item.Functionality.Module.Active) continue;

                if (model.Menus.Any(x => x.Module == item.Functionality.Module.Code && x.Functionality == item.Functionality.Code)) continue;

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
            model.IsManagementReportDelegate = roleManager.IsManagementReportDelegate();
            model.IsDaf = userService.HasDafGroup();
            model.IsGaf = userService.HasGafGroup();
            model.IsCdg = userService.HasCdgGroup();
            model.IsRrhh = userService.HasRrhhGroup();
            model.IsRecruiter = roleManager.IsRecruiter(emailConfig.RecruitersCode);
            model.IsCompliance = userService.HasComplianceGroup();
            model.DafMail = GetGroupMail(emailConfig.DafCode);
            model.CdgMail = GetGroupMail(emailConfig.CdgCode);
            model.PmoMail = GetGroupMail(emailConfig.PmoCode);
            model.RrhhMail = GetGroupMail(emailConfig.RrhhCode);
            model.SellerMail = GetGroupMail(emailConfig.SellerCode);
            model.AreaIds = areaData.GetIdByCurrent();
            model.SectorIds = sectorData.GetIdByCurrent();

            var delegations = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(userData.GetCurrentUser().Id, DelegationType.RefundAdd);
            
            foreach (var userDelegate in delegations)
            {
                model.RefundDelegates.Add(new Option
                {
                    Id = userDelegate.UserId,
                    Text = userDelegate.User.Name
                });
            }
            
            if (delegations.Count == 0)
            {
                var delegationItem = unitOfWork.DelegationRepository.GetByGrantedUserIdAndTypeDelegation(userData.GetCurrentUser().Id);

                if(delegationItem.Count != 0)
                {
                    model.Menus.Add(new MenuModel
                    {
                        Functionality = "NEW-HITO",
                        Module = "SOLFA"
                    });
                }
                
            }
            
            return new Response<MenuResponseModel> { Data = model };
        }
        
        public string GetGroupMail(string code)
        {
            return unitOfWork.GroupRepository.GetEmail(code);
        }
    }
}
