using System.Net.Http;
using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.Service.Http.Interfaces
{
    public interface IBaseHttpClient<T> where T : class
    {
        Result<T> Post(string urlPath, HttpContent content);

        Result<T> Get(string urlPath, string token = null);

        Result<List<T>> GetMany(string urlPath);
    }
}
