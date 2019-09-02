using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IOptionService<TEntity> where TEntity : Option
    {
        Response<string> Add(string description, Dictionary<string, string> parameters);
        Response Update(int id, string description, Dictionary<string, string> parameters);
        Response Active(int id, bool active);
        Response<IList<TEntity>> Get();
        Response<IList<TEntity>> GetActives();
    }
}
