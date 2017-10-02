using System.Net.Http;
using Sofco.Common.Domains;

namespace Sofco.Service.Http.Interfaces
{
    public interface IBaseHttpClient<T> where T : class
    {
        Result<T> Post(string urlPath, HttpContent content);
    }
}
