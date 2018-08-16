using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.AzureAd;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Jobs
{
    public class AzureAddUsersJobService : IAzureAddUsersJobService
    {
        private readonly IAzureService azureService;

        private readonly IUnitOfWork unitOfWork;

        private readonly AppSetting appSetting;

        private readonly EmailConfig emailConfig;

        private List<string> UsersToExcludeForContains => new List<string> { "SHARE", "ADMI", "ALERTA", "USER" };
        private List<string> UsersToExcludeForEquals => new List<string> { "DAF", "IDAP_BIND" };
        private List<string> UsersToExcludeForStartWith => new List<string> { "AZ", "BNP", "BNRD", "BNSH", "CARDIF", "EMPLEO", "ESCUEL",
                                                                              "HEALTH", "HELP", "ISO9", "LABORATO", "KLMDM", "LINKEDIN",
                                                                              "OXIGENO", "PMO", "RECURSOS", "REBRANDI", "RELACIONES", "RRHH",
                                                                              "SEGUIMIENTO", "SOPORTE", "SOFCOAR", "SOFRE", "SRTI", "TFS",
                                                                              "CIENA", "ZABBIX", "AVAYA", "COMPRAS", "COMUNICACION" };

        public AzureAddUsersJobService(IAzureService azureService, 
            IUnitOfWork unitOfWork, 
            IOptions<EmailConfig> emailOptions,
            IOptions<AppSetting> appSetting)
        {
            this.unitOfWork = unitOfWork;
            this.azureService = azureService;
            this.appSetting = appSetting.Value;
            this.emailConfig = emailOptions.Value;
        }

        public void UpdateUsersFromAzureAd()
        {
            var response = GetAllUsers();

            var guestGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.GuestCode);

            foreach (var user in response.Data.Value)
            {
                if (!user.UserPrincipalName.Contains($"@{appSetting.Domain}")) continue;

                if(UsersToExcludeForContains.Any(x => x.Contains(user.UserPrincipalName))) continue;

                if(UsersToExcludeForEquals.Any(x => x.Equals(user.UserPrincipalName))) continue;

                if(UsersToExcludeForStartWith.Any(x => x.StartsWith(user.UserPrincipalName))) continue;

                if (unitOfWork.UserRepository.ExistByMail(user.UserPrincipalName)) continue;

                var domain = SaveUser(user);

                SetGuestGroup(domain, guestGroup);
            }

            unitOfWork.Save();
        }

        private User SaveUser(AzureAdUserResponse user)
        {
            var domain = new User
            {
                Email = user.UserPrincipalName,
                Name = user.DisplayName,
                UserName = user.UserPrincipalName.Split('@')[0],
                Active = true,
                StartDate = DateTime.Now
            };

            unitOfWork.UserRepository.Insert(domain);
            return domain;
        }

        private void SetGuestGroup(User domain, Group guestGroup)
        {
            if (guestGroup != null)
            {
                var userGroup = new UserGroup
                {
                    GroupId = guestGroup.Id,
                    UserId = domain.Id
                };

                unitOfWork.UserGroupRepository.Insert(userGroup);
            }
        }

        private Response<AzureAdUserListResponse> GetAllUsers()
        {
            return this.azureService.GetAllUsersActives();
        }
    }
}
