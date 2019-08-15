using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IOptionService<TEntity> where TEntity : class
    {
        Response<string> Add(string description);
        Response Update(int id, string description);
        Response Active(int id, bool active);
        Response<IList<TEntity>> Get();
        Response<IList<TEntity>> GetActives();
    }
}
