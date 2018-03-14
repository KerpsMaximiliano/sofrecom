using System.Collections.Generic;
using System.Net.Http;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services;
using Sofco.Framework.Helpers;
using Sofco.Model.AzureAd;
using Sofco.Model.Enums;
using Sofco.Model.Users;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;
using Sofco.Service.Settings;

namespace Sofco.Service.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly AzureAdConfig azureAdOptions;

        private readonly IUnitOfWork unitOfWork;

        private readonly IBaseHttpClient client;

        private readonly AppSetting appSetting;

        private readonly IMapper mapper;

        public LoginService(IOptions<AzureAdConfig> azureAdOptions, IBaseHttpClient client, IUnitOfWork unitOfWork, IOptions<AppSetting> appSetting, IMapper mapper)
        {
            this.azureAdOptions = azureAdOptions.Value;
            this.client = client;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.appSetting = appSetting.Value;
        }

        public Response<UserTokenModel> Login(UserLogin userLogin)
        {
            var response = new Response<UserTokenModel>();

            var uri = $"https://login.windows.net/{azureAdOptions.Tenant}/oauth2/token?api-version=1.1";

            var password = CryptographyHelper.Decrypt(userLogin.Password);

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", azureAdOptions.GrantType),
                new KeyValuePair<string, string>("client_id", azureAdOptions.ClientId),
                new KeyValuePair<string, string>("resource", azureAdOptions.Audience),
                new KeyValuePair<string, string>("username", $"{userLogin.UserName}{azureAdOptions.Domain}"),
                new KeyValuePair<string, string>("password", password)
             };

            var result = client.Post<AzureAdLoginResponse>(uri, new FormUrlEncodedContent(pairs));

            if (result.HasErrors)
            {
                response.Messages.Add(new Message("common.loginFailed", MessageType.Error));

                return response;
            }

            if (unitOfWork.UserRepository.IsActive($"{userLogin.UserName}@{appSetting.Domain}"))
            {
                response.Data = Translate(result.Data);

                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.User.UserInactive, MessageType.Error));

            return response;
        }

        public Response<UserTokenModel> Refresh(UserLoginRefresh userLoginRefresh)
        {
            var refreshToken = userLoginRefresh.RefreshToken;

            var uri = $"https://login.windows.net/{azureAdOptions.Tenant}/oauth2/token?api-version=1.1";

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", azureAdOptions.ClientId),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
             };

            var azureLoginResult = client.Post<AzureAdLoginResponse>(uri, new FormUrlEncodedContent(pairs));

            var response = new Response<UserTokenModel>();

            if (azureLoginResult.HasErrors)
            {
                response.Messages.Add(new Message("common.loginFailed", MessageType.Error));
                return response;
            }

            response.Data = Translate(azureLoginResult.Data);

            return response;
        }

        public Response<AzureAdUserListResponse> GetUsersFromAzureAdBySurname(string surname)
        {
            var response = new Response<AzureAdUserListResponse>();

            var tokenResponse = GetGraphToken();

            if (tokenResponse.HasErrors())
            {
                response.AddMessages(tokenResponse.Messages);
                return response;
            }

            var graphUri = $"{azureAdOptions.GraphUsersUrl}?$filter=startswith(surname, '{surname}')";

            var result = client.Get<string>(graphUri, tokenResponse.Data.access_token);

            if (result.HasErrors)
            {
                response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
            }
            else
            {
                var data = JsonConvert.DeserializeObject<AzureAdUserListResponse>(result.ResultData.ToString());
                response.Data = new AzureAdUserListResponse();

                response.Data = data;
            }

            return response;
        }

        public Response<AzureAdUserResponse> GetUserFromAzureAdByEmail(string email)
        {
            var response = new Response<AzureAdUserResponse>();

            var tokenResponse = GetGraphToken();

            if (tokenResponse.HasErrors())
            {
                response.AddMessages(tokenResponse.Messages);
                return response;
            }

            var mail = email;

            var graphUri = azureAdOptions.GraphUsersUrl + "/" + mail;

            var result = client.Get<string>(graphUri, tokenResponse.Data.access_token);

            if (result.HasErrors)
            {
                response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
            }
            else
            {
                response.Data = JsonConvert.DeserializeObject<AzureAdUserResponse>(result.ResultData.ToString());
            }

            return response;
        }

        private Response<AzureAdLoginResponse> GetGraphToken()
        {
            var uri = azureAdOptions.GraphTokenUrl;

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", azureAdOptions.ClientSecretId),
                new KeyValuePair<string, string>("scope", azureAdOptions.Scope),
                new KeyValuePair<string, string>("client_secret", azureAdOptions.ClientSecret),
                new KeyValuePair<string, string>("grant_type", azureAdOptions.ClientCredentials)
             };

            var result = client.Post<string>(uri, new FormUrlEncodedContent(pairs));
            var response = new Response<AzureAdLoginResponse>();

            if (result.HasErrors)
            {
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }
            else
            {
                response.Data = JsonConvert.DeserializeObject<AzureAdLoginResponse>(result.ResultData.ToString());
            }

            return response;
        }

        private UserTokenModel Translate(AzureAdLoginResponse azureLogin)
        {
            return mapper.Map<UserTokenModel>(azureLogin);
        }
    }
}
