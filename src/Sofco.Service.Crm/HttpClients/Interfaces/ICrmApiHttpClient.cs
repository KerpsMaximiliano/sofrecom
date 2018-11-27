using System.Net.Http;
using Sofco.Common.Domains;

namespace Sofco.Service.Crm.HttpClients.Interfaces
{
    public interface ICrmApiHttpClient
    {
        Result<T> Get<T>(string urlPath);

        Result<T> Post<T>(string urlPath, HttpContent content);

        Result<T> Put<T>(string urlPath, HttpContent content);
    }
}