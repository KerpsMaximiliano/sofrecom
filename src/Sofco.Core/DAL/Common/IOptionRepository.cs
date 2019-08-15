using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.DAL.Common
{
    public interface IOptionRepository<TEntity> where TEntity : Option
    {
        IList<TEntity> GetAll();
        TEntity Get(int id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Save();
    }
}
