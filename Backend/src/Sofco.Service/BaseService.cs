using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using System.Collections.Generic;
using System;

namespace Sofco.Service
{
    public abstract class BaseService<T> : IBaseService<T> where T : class
    {
        #region Fields

        protected IBaseRepository<T> _repositoryBase;
        public string currentUser = "";
        public const int RESPONSE_OK = 200;
        public const int RESPONSE_GENERIC_ERROR = 400;
        public const int RESPONSE_UNAUTHORIZED = 401;
        public const int RESPONSE_DUPLICATE_ERROR = 503;

        //public static object HttpContext { get; private set; }

        #endregion

        #region Public methods

        public BaseService(IBaseRepository<T> repository)
        {
            _repositoryBase = repository;
        }

        public IList<T> GetAll()
        {
            return _repositoryBase.GetAll();
        }

        public void Insert(IList<T> entities)
        {
            _repositoryBase.Insert(entities);
            _repositoryBase.Save(currentUser);
        }

        public void Insert(T entity)
        {
            _repositoryBase.Insert(entity);
            _repositoryBase.Save(currentUser);
        }

        public void Update(T entity)
        {
            _repositoryBase.Update(entity);
            _repositoryBase.Save(currentUser);
        }

        public void Update(IList<T> entities)
        {
            _repositoryBase.Update(entities);
            _repositoryBase.Save(currentUser);
        }

        public void Delete(T entity)
        {
            _repositoryBase.Delete(entity);
            _repositoryBase.Save(currentUser);
        }

        public void Delete(IList<T> entities)
        {
            _repositoryBase.Delete(entities);
            _repositoryBase.Save(currentUser);
        }

        public void InsertWithTransaction(T entity)
        {
            _repositoryBase.Insert(entity);
        }

        public void UpdateWithTransaction(T entity)
        {
            _repositoryBase.Update(entity);
        }

        public void BeginTransaction()
        {
            _repositoryBase.BeginTransaction();
        }

        public void Rollback()
        {
            _repositoryBase.Rollback();
        }

        public void Commit(string nombreUsuario)
        {
            _repositoryBase.Commit(nombreUsuario);
        }

        #endregion
    }
}


