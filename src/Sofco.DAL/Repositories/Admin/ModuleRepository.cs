using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class ModuleRepository : BaseRepository<Module>, IModuleRepository
    {
        public ModuleRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return context.Set<Module>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Module> GetAllActivesReadOnly()
        {
            return context.Set<Module>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public IList<Module> GetAllWithFunctionalitiesReadOnly()
        {
            return context.Modules.Include(x => x.Functionalities).ToList().AsReadOnly();
        }

        public Module GetSingleWithFunctionalities(Expression<Func<Module, bool>> predicate)
        {
            return context.Set<Module>()
                .Include(x => x.Functionalities)
                .SingleOrDefault(predicate);
        }
    }
}
