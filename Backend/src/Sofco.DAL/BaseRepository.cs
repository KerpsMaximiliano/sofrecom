using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Sofco.DAL.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        #region Fields

        protected readonly SofcoContext _context;
        protected IDbContextTransaction _contextTransaction;

        #endregion

        #region Constructors

        public BaseRepository(SofcoContext context)
        {
            _context = context;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Retorna todas las entidades.
        /// </summary>
        public IList<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public virtual IList<T> GetAllReadOnly()
        {
            return _context.Set<T>().ToList().AsReadOnly();
        }

        /// <summary>
        /// Retorna una unica entidad de acuerdo al criterio
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T GetSingle(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().SingleOrDefault(predicate);
        }

        /// <summary>
        /// Inserta una entidad.
        /// </summary>
        public void Insert(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        /// <summary>
        /// Inserta multiples entidades.
        /// </summary>
        public void Insert(IList<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        /// <summary>
        /// Actualiza una entidad
        /// </summary>
        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        /// <summary>
        ///  Actualiza multiple entidades
        /// </summary>
        public void Update(IList<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        /// <summary>
        /// Borra una entidad.
        /// </summary>
        public void Delete(T entity)
        {
            ILogicalDelete logicalDelete = entity as ILogicalDelete;
            if (logicalDelete == null)
            {
                _context.Set<T>().Remove(entity);
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
            return _context.Set<T>().Where(predicate).ToList();
        }

        /// <summary>
        /// Graba todos los cambios realizados en el contexto.
        /// </summary>
        public void Save(string nombreUsuario)
        {
            // Genera la auditoria.
            /* foreach (var entry in context.ChangeTracker.Entries().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
             {
                 context.Set<AuditoriaObjeto>().AddRange(AuditRecords(entry, nombreUsuario));
             }*/
            _context.SaveChanges();
        }

        public void BeginTransaction()
        {
            _contextTransaction = _context.Database.BeginTransaction();
        }

        public void Rollback()
        {
            if (_contextTransaction != null)
                _contextTransaction.Rollback();
        }

        public void Commit(string nombreUsuario)
        {
            Save(nombreUsuario);
            if (_contextTransaction != null)
            {
                _contextTransaction.Commit();
            }
        }

        #endregion
    }
}
