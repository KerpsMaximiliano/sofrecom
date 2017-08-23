using Sofco.Core.DAL;
using Sofco.Model.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories
{
    public class FunctionalityRepository : BaseRepository<Functionality>, IFunctionalityRepository
    {
        public FunctionalityRepository(SofcoContext context) : base(context)
        {
        }

        public bool ExistById(int id)
        {
            return _context.Set<Functionality>().Any(x => x.Id == id);
        }

        /// <summary>
        /// Retorna todas las entidades en modo solo lectura
        /// </summary>
        public IList<Functionality> GetAllActivesReadOnly()
        {
            return _context.Set<Functionality>().Where(x => x.Active).ToList().AsReadOnly();
        }
    }
}
