using System;
using System.Net.Http;
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

        private Result<T> GetResult(HttpRequestMessage requestMessage)
        {
            var responseMessage = httpClient.SendAsync(requestMessage).Result;

            var result = new Result<T>();

            if (!responseMessage.IsSuccessStatusCode)
            {
                result.AddError(responseMessage.ReasonPhrase);

                return result;
            }

            result.ResultData = responseMessage.Content.ReadAsStringAsync().Result;

            return result;
        }

        public Result<T> Post(string urlPath, HttpContent content)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Post);

            requestMessage.Content = content;

            return GetResult(requestMessage);
        }
    }
}