using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Repositories.Common
{
    public class OptionRepository<TEntity> : IOptionRepository<TEntity> where TEntity : Option
    {
        private readonly SofcoContext context;

        public OptionRepository(SofcoContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Retorna todas las entidades.
        /// </summary>
        public IList<TEntity> GetAll()
        {
            return context.Set<TEntity>().ToList();
        }

        public TEntity Get(int id)
        {
            return context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// Inserta una entidad.
        /// </summary>
        public void Insert(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Actualiza una entidad
        /// </summary>
        public void Update(TEntity entity)
        {
            context.Set<TEntity>().Update(entity);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public TEntity GetByDescription(string description)
        {
            return context.Set<TEntity>().SingleOrDefault(x => x.Text.ToLowerInvariant().Equals(description.ToLowerInvariant()));
        }
    }
}
