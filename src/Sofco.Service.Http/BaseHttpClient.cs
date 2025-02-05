﻿using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sofco.Common.Domains;
using Sofco.Service.Http.Extensions;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Http
{
    public class BaseHttpClient : IBaseHttpClient
    {
        private const string ErrorDelimiter = " - ";

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

        private Result<TResult> GetResult<TResult>(HttpRequestMessage requestMessage)
        {
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

            return jsonSerializerSettings == null ?
                JsonConvert.DeserializeObject<TResult>(resultText)
                : JsonConvert.DeserializeObject<TResult>(resultText, jsonSerializerSettings);
        }

        public Result<T> Post<T>(string urlPath, HttpContent content)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Post);

            requestMessage.Content = content;

            return GetResult<T>(requestMessage);
        }

        public Result<T> Put<T>(string urlPath, HttpContent content)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Put);

            requestMessage.Content = content;

            return GetResult<T>(requestMessage);
        }

        public Result<T> Get<T>(string urlPath, string token = null, TimeSpan? timeOut = null)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Get);

            if (timeOut != null)
            {
                requestMessage.SetTimeout(timeOut);
            }

            if (!string.IsNullOrEmpty(token))
            {
                requestMessage.Headers.Add("Authorization", token);
            }

            return GetResult<T>(requestMessage);
        }

        public Result<List<T>> GetMany<T>(string urlPath, string token = null)
        {
            var requestMessage = BuildRequest(urlPath, HttpMethod.Get);

            if (!string.IsNullOrEmpty(token))
            {
                requestMessage.Headers.Add("Authorization", "Bearer " + token);
            }

            return GetResult<List<T>>(requestMessage);
        }
    }
}