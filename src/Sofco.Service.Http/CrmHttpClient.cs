using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Http
{
    public class CrmHttpClient : ICrmHttpClient
    {
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
                throw new Exception(string.Join("; ", result.Errors));
            }

            return result;
        }
    }
}
