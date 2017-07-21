using System.Collections.Generic;

namespace Sofco.Core.Interfaces.Services
{
    public interface IBaseService<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllReadOnly();
        void Insert(T entity);
        void Insert(IEnumerable<T> entities);
        void Update(T entityo);
        void Update(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);

        void InsertWithTransaction(T entity);
        void UpdateWithTransaction(T entity);
        void BeginTransaction();
        void Rollback();
        void Commit(string nombreUsuario);
    }
}
