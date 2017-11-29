using System;
using System.Collections.Generic;
using System.Net.Http;
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
    }
}
