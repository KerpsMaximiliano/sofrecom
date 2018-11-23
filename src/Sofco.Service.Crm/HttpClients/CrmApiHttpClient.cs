using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sofco.Common.Domains;
using Sofco.Service.Crm.HttpClients.Interfaces;

namespace Sofco.Service.Crm.HttpClients
{
    public class CrmApiHttpClient : ICrmApiHttpClient
    {
        private const string ErrorDelimiter = ";";

        private const string Resource = "https://sofrecomdev.crm2.dynamics.com";

        private const string ClientId = "5979a783-4e8a-42a6-acb9-c5f20da86520";

        private const string ClientSecret = "gmup69BdV0BVvqtoRGZCQIX5ULXtSVEpX3mCItjS+AM=";

        private readonly string accessToken = string.Empty;

        private readonly HttpClient httpClient;

        private readonly string baseAddress = "https://sofrecomdev.api.crm2.dynamics.com/api/data/v9.0/";

        public CrmApiHttpClient()
        {
            if (accessToken == string.Empty)
            {
                accessToken = GetAccessToken();
            }

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Add("Prefer", "odata.include-annotations=\"OData.Community.Display.V1.FormattedValue\"");
        }

        public Result<T> Get<T>(string urlPath)
        {
            return GetResult<T>(urlPath, HttpMethod.Get);
        }

        public Result<List<T>> GetMany<T>(string urlPath)
        {
            return GetResult<List<T>>(urlPath, HttpMethod.Get);
        }

        public Result<T> Post<T>(string urlPath, HttpContent content)
        {
            return GetResult<T>(urlPath, HttpMethod.Post);
        }

        public Result<T> Put<T>(string urlPath, HttpContent content)
        {
            return GetResult<T>(urlPath, HttpMethod.Put);
        }

        private string GetAccessToken()
        {
            var webApiUrl = new Uri("https://login.microsoftonline.com/31d7c510-2e1a-4b74-b0fe-8996a7a23a4d/oauth2/token");

            var requestToken = new HttpClient();

            var pairs = new Dictionary<string, string>
            {
                {"Resource", Resource},
                {"client_id", ClientId},
                {"client_secret", ClientSecret},
                {"grant_type", "client_credentials"}
            };

            var formContent = new FormUrlEncodedContent(pairs);

            var response = requestToken.PostAsync(webApiUrl, formContent).Result;

            var jsonResponse = response.Content.ReadAsStringAsync().Result;

            var token = JObject.Parse(jsonResponse).SelectToken("access_token");

            return token.ToString();
        }

        private Result<TResult> GetResult<TResult>(string urlPath, HttpMethod httpMethod)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, baseAddress + urlPath);

            var response = httpClient.SendAsync(requestMessage).Result;

            if (!response.IsSuccessStatusCode)
            {
                var result = new Result<TResult>();

                result.AddError(requestMessage.RequestUri
                                + ErrorDelimiter +
                                response.ReasonPhrase
                                + ErrorDelimiter +
                                response.Content.ReadAsStringAsync().Result);

                return result;
            }

            var resultData = GetResponseResult<TResult>(response);

            return new Result<TResult>(resultData);
        }

        protected TResult GetResponseResult<TResult>(HttpResponseMessage response, JsonSerializerSettings jsonSerializerSettings = null)
        {
            var resultText = response.Content.ReadAsStringAsync().Result;

            if (typeof(TResult) == typeof(string))
            {
                return (TResult)(object)resultText;
            }

            if (typeof(TResult) == typeof(JObject))
            {
                return (TResult)(object)JObject.Parse(resultText);
            }

            return jsonSerializerSettings == null ?
                JsonConvert.DeserializeObject<TResult>(resultText)
                : JsonConvert.DeserializeObject<TResult>(resultText, jsonSerializerSettings);
        }
    }
}
