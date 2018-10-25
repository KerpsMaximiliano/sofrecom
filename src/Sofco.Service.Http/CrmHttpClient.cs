using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Sofco.Common.Domains;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Http
{
    public class CrmHttpClient : ICrmHttpClient
    {
        private const string ErrorDelimiter = ";";

        private readonly IBaseHttpClient client;

        public CrmHttpClient(IBaseHttpClient client)
        {
            this.client = client;
        }

        public Result<List<T>> GetMany<T>(string urlPath)
        {
            var result = client.GetMany<T>(urlPath);

            if (result.HasErrors)
            {
                throw new Exception(string.Join(ErrorDelimiter, result.Errors));
            }

            return result;
        }

        public Result<T> Get<T>(string urlPath)
        {
            var result = client.Get<T>(urlPath);

            if (result.HasErrors)
            {
                throw new Exception(string.Join(ErrorDelimiter, result.Errors));
            }

            return result;
        }

        public Result<T> Post<T>(string urlPath, HttpContent content)
        {
            var result = client.Post<T>(urlPath, content);

            if (result.HasErrors)
            {
                throw new Exception(string.Join(ErrorDelimiter, result.Errors));
            }

            return result;
        }

        public Result<T> Put<T>(string urlPath, HttpContent content)
        {
            var result = client.Put<T>(urlPath, content);

            if (result.HasErrors)
            {
                throw new Exception(string.Join(ErrorDelimiter, result.Errors));
            }

            return result;
        }

        public void GetAccessToken()
        {
            string organizationUrl = "https://sofrecomdev.api.crm2.dynamics.com";
            string clientId = "5979a783-4e8a-42a6-acb9-c5f20da86520";
            string appKey = "gmup69BdV0BVvqtoRGZCQIX5ULXtSVEpX3mCItjS+AM=";
            string aadInstance = "https://login.microsoftonline.com/";
            string tenantID = "31d7c510-2e1a-4b74-b0fe-8996a7a23a4d";

            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance + tenantID);
            AuthenticationResult authenticationResult = authenticationContext.AcquireTokenAsync(organizationUrl, clientcred).Result;
            var accessToken = authenticationResult.AccessToken;

            var customers = client.GetMany<string>("https://sofrecomdev.api.crm2.dynamics.com/api/data/v9.0/opportunities", accessToken).ResultData;
        }
    }
}
