﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sofco.Core.Interfaces.DAL
{
    public interface IBaseRepository<T> where T : class
    {
        IList<T> GetAll();
        IList<T> GetAllReadOnly();
        T GetSingle(Expression<Func<T, bool>> predicate);
        void Insert(T entity);
        void Insert(IList<T> entities);
        void Update(T entityo);
        void Update(IList<T> entities);
        void Delete(T entity);
        void Delete(IList<T> entities);

        IList<T> Where(Expression<Func<T, bool>> predicate);

        void BeginTransaction();
        void Rollback();
        void Commit(string nombreUsuario);
        void Save(string nombreUsuario);
    }
}
