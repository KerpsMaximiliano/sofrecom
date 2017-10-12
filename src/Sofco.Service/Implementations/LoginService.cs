using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Sofco.Core.Services;
using Sofco.Common.Domains;
using Sofco.Model.Users;
using Sofco.Service.Settings;
using Sofco.Service.Http.Interfaces;
using Newtonsoft.Json;
using Sofco.Model.AzureAd;
using Sofco.Model.Utils;
using Sofco.Model.Enums;

namespace Sofco.Service.Implementations
{
    public class LoginService : ILoginService
    {
        private readonly AzureAdConfig azureAdOptions;

        private readonly IBaseHttpClient<string> client;

        public LoginService(IOptions<AzureAdConfig> azureAdOptions, IBaseHttpClient<string> client)
        {
            this.azureAdOptions = azureAdOptions.Value;
            this.client = client;
        }

        public Result Login(UserLogin userLogin)
        {
            var uri = $"https://login.windows.net/{azureAdOptions.Tenant}/oauth2/token?api-version=1.1";

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", azureAdOptions.GrantType),
                new KeyValuePair<string, string>("client_id", azureAdOptions.ClientId),
                new KeyValuePair<string, string>("resource", azureAdOptions.Audience),
                new KeyValuePair<string, string>("username", $"{userLogin.UserName}@tebrasofre.onmicrosoft.com"),
                new KeyValuePair<string, string>("password", userLogin.Password)
             };

            return client.Post(uri, new FormUrlEncodedContent(pairs));
        }

        public Result Refresh(UserLoginRefresh userLoginRefresh)
        {
            var refreshToken = userLoginRefresh.RefreshToken;

            var uri = $"https://login.windows.net/{azureAdOptions.Tenant}/oauth2/token?api-version=1.1";

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("client_id", azureAdOptions.ClientId),
                new KeyValuePair<string, string>("refresh_token", refreshToken)
             };

            return client.Post(uri, new FormUrlEncodedContent(pairs));
        }


        public Response<AzureAdUserListResponse> GetUsersFromAzureADBySurname(string surname)
        {
            var response = new Response<AzureAdUserListResponse>();

            var tokenResponse = GetGraphToken();

            if (tokenResponse.HasErrors())
            {
                response.AddMessages(tokenResponse.Messages);
                return response;
            }

            var graphUri = $"{azureAdOptions.GraphUsersUrl}?$filter=startswith(surname, '{surname}')";

            var result = client.Get(graphUri, tokenResponse.Data.access_token);

            if (result.HasErrors)
            {
                response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
            }
            else
            {
                var data = JsonConvert.DeserializeObject<AzureAdUserListResponse>(result.ResultData.ToString());
                response.Data = new AzureAdUserListResponse();

                foreach (var item in data.Value)
                {
                    item.UserPrincipalName = item.UserPrincipalName.Replace("@tebrasofre.onmicrosoft.com", "@sofrecom.com.ar");
                }

                response.Data = data;
            }

            return response;
        }

        public Response<AzureAdUserResponse> GetUserFromAzureADByEmail(string email)
        {
            var response = new Response<AzureAdUserResponse>();

            var tokenResponse = GetGraphToken();

            if (tokenResponse.HasErrors())
            {
                response.AddMessages(tokenResponse.Messages);
                return response;
            }

            var mail = email;
            var username = string.Empty;

            if (email.Contains("@sofrecom.com.ar"))
            {
                username = email.Split('@')[0];
                mail = $"{username}@tebrasofre.onmicrosoft.com";
            }

            var graphUri = azureAdOptions.GraphUsersUrl + "/" + mail;

            var result = client.Get(graphUri, tokenResponse.Data.access_token);

            if (result.HasErrors)
            {
                response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
            }
            else
            {
                response.Data = JsonConvert.DeserializeObject<AzureAdUserResponse>(result.ResultData.ToString());
                response.Data.UserPrincipalName = response.Data.UserPrincipalName.Replace("@tebrasofre.onmicrosoft.com", "@sofrecom.com.ar");
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

            var result = client.Post(uri, new FormUrlEncodedContent(pairs));
            var response = new Response<AzureAdLoginResponse>();

            if (result.HasErrors)
            {
                response.Messages.Add(new Message(Resources.es.Common.GeneralError, MessageType.Error));
            }
            else
            {
                response.Data = JsonConvert.DeserializeObject<AzureAdLoginResponse>(result.ResultData.ToString());
            }

            return response;
        }
    }
}
