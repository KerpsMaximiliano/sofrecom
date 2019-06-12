using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Repositories.Common
{
    public class CurrencyExchangeRepository : BaseRepository<CurrencyExchange>, ICurrencyExchangeRepository
    {
        public CurrencyExchangeRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int idValue)
        {
            return context.CurrencyExchanges.Any(x => x.Id == idValue);
        }

        public IList<CurrencyExchange> Get(DateTime startDate, DateTime endDate)
        {
            return context.CurrencyExchanges.Include(x => x.Currency).Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date).ToList();
        }
    }
}
