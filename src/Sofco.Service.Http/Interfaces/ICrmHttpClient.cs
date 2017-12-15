using System.Collections.Generic;
using System.Net.Http;
using Sofco.Common.Domains;

namespace Sofco.Service.Http.Interfaces
{
    public interface ICrmHttpClient
    {
        Result<T> Get<T>(string urlPath);

        Result<List<T>> GetMany<T>(string urlPath);

        Result<T> Post<T>(string urlPath, HttpContent content);

        Result<T> Put<T>(string urlPath, HttpContent content);
    }
}