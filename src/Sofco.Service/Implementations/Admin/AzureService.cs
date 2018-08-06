using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Common.Settings;
using Sofco.Core.Services.Admin;
using Sofco.Domain.AzureAd;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Http.Interfaces;
using Sofco.Service.Settings;

namespace Sofco.Service.Implementations.Admin
{
    public class AzureService : IAzureService
    {
        private readonly AzureAdConfig azureAdOptions;

        private readonly AppSetting appSetting;

        private readonly IBaseHttpClient client;

        public AzureService(IOptions<AzureAdConfig> azureAdOptions, IBaseHttpClient client, IOptions<AppSetting> appSetting)
        {
            this.azureAdOptions = azureAdOptions.Value;
            this.client = client;
            this.appSetting = appSetting.Value;
        }

        public Response<AzureAdUserListResponse> GetAllUsersActives()
        {
            var response = new Response<AzureAdUserListResponse>();

            var tokenResponse = GetGraphToken();

            if (tokenResponse.HasErrors())
            {
                response.AddMessages(tokenResponse.Messages);
                return response;
            }

            var graphUri = $"{azureAdOptions.GraphUsersUrl}?$filter=accountEnabled eq true";

            var result = client.Get<string>(graphUri, tokenResponse.Data.access_token);

            var data = JsonConvert.DeserializeObject<AzureAdUserListResponse>(result.ResultData.ToString());
            response.Data = new AzureAdUserListResponse();

            var end = false;

            while (!end)
            {
                if (string.IsNullOrWhiteSpace(data.NextLink)) end = true;

                foreach (var azureAdUserResponse in data.Value)
                {
                    if (azureAdUserResponse.UserPrincipalName.Contains($"@{appSetting.Domain}"))
                    {
                        response.Data.Value.Add(azureAdUserResponse);
                    }
                }

                if (!end)
                {
                    result = client.Get<string>(data.NextLink, tokenResponse.Data.access_token);
                    data = JsonConvert.DeserializeObject<AzureAdUserListResponse>(result.ResultData.ToString());
                }
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
    }
}

