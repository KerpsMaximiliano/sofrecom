using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Repository.Rh.Repositories
{
    public class TigerPrepaidHealthRepository : ITigerPrepaidHealthRepository
    {
        private DbSet<TigerPrepaidHealth> TigerPrepaidHealthSet { get; }

        public TigerPrepaidHealthRepository(TigerContext context)
        {
            TigerPrepaidHealthSet = context.Set<TigerPrepaidHealth>();
        }

        public List<TigerPrepaidHealth> GetAll()
        {
            return TigerPrepaidHealthSet.ToList();
        }
    }
}
