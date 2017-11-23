using System.Collections.Generic;
using System.Net.Http;
using Sofco.Common.Domains;

namespace Sofco.Service.Http.Interfaces
{
    public interface ICrmHttpClient
    {
        Result<List<T>> GetMany<T>(string urlPath);

        Result<T> Post<T>(string urlPath, StringContent stringContent);

        Result<T> Put<T>(string urlPath, StringContent stringContent);
    }
}