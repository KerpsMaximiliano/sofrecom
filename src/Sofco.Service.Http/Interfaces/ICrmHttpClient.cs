using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.Service.Http.Interfaces
{
    public interface ICrmHttpClient
    {
        Result<List<T>> GetMany<T>(string urlPath);
    }
}