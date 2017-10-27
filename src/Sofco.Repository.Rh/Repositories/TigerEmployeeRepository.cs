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
        protected readonly TigerContext context;

        private DbSet<TigerEmployee> tigerEmployeeSet { get; set; }

        public TigerEmployeeRepository(TigerContext context)
        {
            tigerEmployeeSet = context.Set<TigerEmployee>();
        }

        public IList<TigerEmployee> GetAll()
        {
            return tigerEmployeeSet.ToList();
        }

        public IList<TigerEmployee> GetWithStartDate(DateTime startDate)
        {
            return tigerEmployeeSet
                .Where(s => s.Feiem >= startDate).ToList();
        }

        public IList<TigerEmployee> GetWithEndDate(DateTime endDate)
        {
            return tigerEmployeeSet
                .Where(s => s.Febaj != null && s.Febaj >= endDate).ToList();
        }
    }
}
