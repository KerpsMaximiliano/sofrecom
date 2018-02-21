using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Repository.Rh.Repositories
{
    public class TigerHealthInsuranceRepository : ITigerHealthInsuranceRepository
    {
        private DbSet<TigerHealthInsurance> TigerHealthInsuranceSet { get; }

        public TigerHealthInsuranceRepository(TigerContext context)
        {
            TigerHealthInsuranceSet = context.Set<TigerHealthInsurance>();
        }

        public List<TigerHealthInsurance> GetAll()
        {
            return TigerHealthInsuranceSet.ToList();
        }
    }
}
