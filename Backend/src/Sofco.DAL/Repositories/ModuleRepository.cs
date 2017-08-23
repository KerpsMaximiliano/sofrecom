using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sofco.Core.DAL;
using Sofco.Model.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Sofco.DAL.Repositories
{
    public class ModuleRepository : BaseRepository<Module>, IModuleRepository
    {
        public ModuleRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return _context.Set<Module>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Module> GetAllActivesReadOnly()
        {
            return _context.Set<Module>().Where(x => x.Active).ToList().AsReadOnly();
        }

        public Module GetSingleWithFunctionalities(Expression<Func<Module, bool>> predicate)
        {
            return _context.Set<Module>()
                .Include(x => x.ModuleFunctionality)
                    .ThenInclude(s => s.Functionality)
                .SingleOrDefault(predicate);
        }
    }
}
