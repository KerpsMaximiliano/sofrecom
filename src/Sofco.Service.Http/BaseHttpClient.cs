using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sofco.Common.Domains;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Http
{
    public class BaseHttpClient<T> : IBaseHttpClient<T> where T : class
    {
        private const string ErrorResponseMessage = "Error";

        private readonly HttpClient httpClient;

        public BaseHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        private HttpRequestMessage BuildRequest(string urlPath, HttpMethod verb)
        {
            var request = new HttpRequestMessage
            {
                Method = verb,
                RequestUri = new Uri(urlPath)
            };

            return request;
        }

        private Result<TResult> GetResult<TResult>(HttpRequestMessage requestMessage) where TResult : class
        {
            var response = httpClient.SendAsync(requestMessage).Result;

            var result = new Result<TResult>();

            if (!response.IsSuccessStatusCode)
            {
                result.AddError(response.ReasonPhrase);

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

            try
            {
                return jsonSerializerSettings == null ?
                    JsonConvert.DeserializeObject<TResult>(resultText)
                    : JsonConvert.DeserializeObject<TResult>(resultText, jsonSerializerSettings);
            }
            catch (JsonSerializationException ex)
            {
                // TODO: Implement logger
                // logger.LogError($"{ex.Message} | Response: {resultText}");
                throw;
            }
        }

        public Result<T> Post(string urlPath, HttpContent content)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Post);

            requestMessage.Content = content;

            return GetResult<T>(requestMessage);
        }

        public Result<T> Get(string urlPath, string token = null)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Get);

            if(!string.IsNullOrEmpty(token))
            {
                requestMessage.Headers.Add("Authorization", token);
            }

            return GetResult<T>(requestMessage);
        }

        public Result<List<T>> GetMany(string urlPath)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Get);

            return GetResult<List<T>>(requestMessage);
        }
    }
}