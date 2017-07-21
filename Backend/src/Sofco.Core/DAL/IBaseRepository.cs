using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sofco.Core.Interfaces.DAL
{
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllReadOnly();
        T GetSingle(Expression<Func<T, bool>> predicate);
        void Insert(T entity);
        void Insert(IEnumerable<T> entities);
        void Update(T entityo);
        void Update(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);

        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);

        void BeginTransaction();
        void Rollback();
        void Commit(string nombreUsuario);
        void Save(string nombreUsuario);
    }
}
