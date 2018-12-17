using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sofco.Common.Domains;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Settings;

namespace Sofco.Service.Crm.HttpClients
{
    public class CrmApiHttpClient : ICrmApiHttpClient
    {
        private const string JsonMediaType = "application/json";

        private const string ErrorDelimiter = ";";

        private readonly CrmSetting crmSetting;

        private readonly string accessToken = string.Empty;

        private readonly HttpClient httpClient;

        public CrmApiHttpClient(IOptions<CrmSetting> crmOptions)
        {
            crmSetting = crmOptions.Value;

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

        public Result<T> Post<T>(string urlPath, object data)
        {
            return GetResult<T>(urlPath, HttpMethod.Post, data);
        }

        public Result<T> Put<T>(string urlPath, object data)
        {
            return GetResult<T>(urlPath, HttpMethod.Put, data);
        }

        public Result<T> Patch<T>(string urlPath, object data)
        {
            return GetResult<T>(urlPath, new HttpMethod("PATCH"), data);
        }

        private string GetAccessToken()
        {
            var webApiUrl = new Uri(crmSetting.UrlApiToken);

            var requestToken = new HttpClient();

            var pairs = new Dictionary<string, string>
            {
                {"Resource", crmSetting.Resource},
                {"client_id", crmSetting.ClientId},
                {"client_secret", crmSetting.ClientSecret},
                {"grant_type", "client_credentials"}
            };

            var formContent = new FormUrlEncodedContent(pairs);

            var response = requestToken.PostAsync(webApiUrl, formContent).Result;

            var jsonResponse = response.Content.ReadAsStringAsync().Result;

            var token = JObject.Parse(jsonResponse).SelectToken("access_token");

            return token.ToString();
        }

        private Result<TResult> GetResult<TResult>(string urlPath, HttpMethod httpMethod, object data = null)
        {
            var requestMessage = new HttpRequestMessage(httpMethod, crmSetting.UrlApi + urlPath);

            if (data != null)
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, JsonMediaType);

                requestMessage.Content = content;
            }

            var response = httpClient.SendAsync(requestMessage).Result;

            response = ProcessExpiredToken(response);

            if (!response.IsSuccessStatusCode)
            {
                var result = new Result<TResult>();

                result.AddError(requestMessage.RequestUri
                                + ErrorDelimiter +
                                response.ReasonPhrase
                                + ErrorDelimiter +
                                response.Content.ReadAsStringAsync().Result);

                throw new Exception(string.Join(ErrorDelimiter, result.Errors));
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new Result<TResult>();
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

        private HttpResponseMessage ProcessExpiredToken(HttpResponseMessage unauthorizedResponse)
        {
            if(unauthorizedResponse.IsSuccessStatusCode) return unauthorizedResponse;

            if(unauthorizedResponse.StatusCode != HttpStatusCode.Unauthorized) return unauthorizedResponse;

            httpClient.DefaultRequestHeaders.Authorization 
                = new AuthenticationHeaderValue("Bearer", GetAccessToken());

            var request = new HttpRequestMessage(
                    unauthorizedResponse.RequestMessage.Method,
                    unauthorizedResponse.RequestMessage.RequestUri);

            return httpClient.SendAsync(request).Result;
        }
    }
}
