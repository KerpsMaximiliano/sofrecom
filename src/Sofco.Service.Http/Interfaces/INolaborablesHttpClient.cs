using Sofco.Common.Domains;

namespace Sofco.Service.Http.Interfaces
{
    public interface INolaborablesHttpClient
    {
        Result<T> Get<T>(string urlPath);
    }
}