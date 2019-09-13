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

        public CurrencyExchange Get(DateTime date, string hitoMoneyId)
        {
            return context.CurrencyExchanges.Include(x => x.Currency)
                .SingleOrDefault(x => x.Date.Month == date.Month && x.Date.Year == date.Year && x.Currency.CrmId.Equals(hitoMoneyId));
        }
    }
}
