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

        public Result<T> Post<T>(string urlPath, StringContent stringContent)
        {
            var result = client.Post<T>(urlPath, stringContent);

            if (result.HasErrors)
            {
                throw new Exception(string.Join(ErrorDelimiter, result.Errors));
            }

            return result;

        }
    }
}
