using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Domain.Rh.Tiger;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Repository.Rh.Repositories
{
    public class TigerEmployeeRepository : ITigerEmployeeRepository
    {
        private DbSet<TigerEmployee> TigerEmployeeSet { get; }

        public TigerEmployeeRepository(TigerContext context)
        {
            TigerEmployeeSet = context.Set<TigerEmployee>();
        }

        public IList<TigerEmployee> GetAll()
        {
            return TigerEmployeeSet.ToList();
        }

        public IList<TigerEmployee> GetWithStartDate(DateTime startDate)
        {
            return TigerEmployeeSet
                .Where(s => s.Feiem >= startDate).ToList();
        }

        public IList<TigerEmployee> GetWithEndDate(DateTime endDate)
        {
            return TigerEmployeeSet
                .Where(s => s.Febaj != null && s.Febaj >= endDate).ToList();
        }
    }
}
