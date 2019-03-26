using Sofco.Common.Domains;

namespace Sofco.Service.Crm.HttpClients.Interfaces
{
    public interface ICrmApiHttpClient
    {
        Result<T> Get<T>(string urlPath);

        Result<T> Post<T>(string urlPath, object data);

        Result<T> Put<T>(string urlPath, object data);

        Result<T> Patch<T>(string urlPath, object data);

        Result<T> Delete<T>(string urlPath, object data);
    }
}