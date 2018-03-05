using System;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Jobs;
using Sofco.Model.AzureAd;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Jobs
{
    public class AzureAddUsersJobService : IAzureAddUsersJobService
    {
        private readonly IAzureService azureService;

        private readonly IUnitOfWork unitOfWork;

        private readonly AppSetting appSetting;

        public AzureAddUsersJobService(IAzureService azureService, IUnitOfWork unitOfWork, IOptions<AppSetting> appSetting)
        {
            this.unitOfWork = unitOfWork;
            this.azureService = azureService;
            this.appSetting = appSetting.Value;
        }

        public void UpdateUsersFromAzureAd()
        {
            var response = GetAllUsers();

            foreach (var user in response.Data.Value)
            {
                if (user.UserPrincipalName.Contains($"@{appSetting.Domain}"))
                {
                    if (!unitOfWork.UserRepository.ExistByMail(user.UserPrincipalName))
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
                    }
                }
            }

            unitOfWork.Save();
        }

        private Response<AzureAdUserListResponse> GetAllUsers()
        {
            return this.azureService.GetAllUsersActives();
        }
    }
}
