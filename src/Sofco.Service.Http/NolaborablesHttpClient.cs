using System;
using Sofco.Common.Domains;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Http
{
    public class NolaborablesHttpClient : INolaborablesHttpClient
    {
        private const string ErrorDelimiter = ";";

        private readonly IBaseHttpClient client;

        public NolaborablesHttpClient(IBaseHttpClient client)
        {
            this.client = client;
        }

        public Result<T> Get<T>(string urlPath)
        {
            var result = client.Get<T>(urlPath, timeOut: TimeSpan.FromMinutes(5));

            if (result.HasErrors)
            {
                throw new Exception(string.Join(ErrorDelimiter, result.Errors));
            }

            return result;
        }
    }
}
