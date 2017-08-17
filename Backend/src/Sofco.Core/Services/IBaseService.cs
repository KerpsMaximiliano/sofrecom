using System.Collections.Generic;

namespace Sofco.Core.Interfaces.Services
{
    public interface IBaseService<T> where T : class
    {
        IList<T> GetAll();
        void Insert(T entity);
        void Insert(IList<T> entities);
        void Update(T entityo);
        void Update(IList<T> entities);
        void Delete(T entity);
        void Delete(IList<T> entities);

        void InsertWithTransaction(T entity);
        void UpdateWithTransaction(T entity);
        void BeginTransaction();
        void Rollback();
        void Commit(string nombreUsuario);
    }
}
