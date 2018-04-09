using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Sofco.Core.DAL.Common;
using Sofco.Common.Domains;

namespace Sofco.DAL.Repositories.Common
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region Fields

        protected readonly SofcoContext context;
        protected IDbContextTransaction contextTransaction;

        #endregion

        #region Constructors

        public BaseRepository(SofcoContext context)
        {
            this.context = context;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Retorna todas las entidades.
        /// </summary>
        public IList<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public virtual IList<T> GetAllReadOnly()
        {
            return context.Set<T>().ToList().AsReadOnly();
        }

        /// <summary>
        /// Retorna una unica entidad de acuerdo al criterio
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().SingleOrDefault(predicate);
        }

        /// <summary>
        /// Inserta una entidad.
        /// </summary>
        public void Insert(T entity)
        {
            if (entity is IEntityDate entityDate)
            {
                entityDate.Created = DateTime.UtcNow;
            }

            context.Set<T>().Add(entity);
        }

        /// <summary>
        /// Inserta multiples entidades.
        /// </summary>
        public void Insert(IList<T> entities)
        {
            foreach(var item in entities)
            {
                if (item is IEntityDate entityDate)
                {
                    entityDate.Created = DateTime.UtcNow;
                }
            }

            context.Set<T>().AddRange(entities);
        }

        /// <summary>
        /// Actualiza una entidad
        /// </summary>
        public void Update(T entity)
        {
            if (entity is IEntityDate entityDate)
            {
                entityDate.Modified = DateTime.UtcNow;
            }

            context.Set<T>().Update(entity);
        }

        /// <summary>
        ///  Actualiza multiple entidades
        /// </summary>
        public void Update(IList<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
        }

        /// <summary>
        /// Borra una entidad.
        /// </summary>
        public virtual void Delete(T entity)
        {
            ILogicalDelete logicalDelete = entity as ILogicalDelete;
            if (logicalDelete == null)
            {
                context.Set<T>().Remove(entity);
            }
            else
            {
                logicalDelete.Active = false;
            }
        }

        /// <summary>
        /// Borra multiples entidades
        /// </summary>
        public void Delete(IList<T> entities)
        {
            foreach (var entity in entities)
                Delete(entity);
        }

        /// <summary>
        /// Retorna una o varias entidades según condición.
        /// </summary>
        public IList<T> Where(Expression<Func<T, bool>> predicate)
        {
            return context.Set<T>().Where(predicate).ToList();
        }

        /// <summary>
        /// Graba todos los cambios realizados en el contexto.
        /// </summary>
        public void Save()
        {
            context.SaveChanges();
        }

        public void BeginTransaction()
        {
            contextTransaction = context.Database.BeginTransaction();
        }

        public void Rollback()
        {
            contextTransaction?.Rollback();
        }

        public void Commit(string nombreUsuario)
        {
            Save();

            contextTransaction?.Commit();
        }

        #endregion
    }
}
