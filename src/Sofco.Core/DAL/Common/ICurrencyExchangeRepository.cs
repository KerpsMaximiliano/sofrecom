using System;
using System.Collections.Generic;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface ICurrencyExchangeRepository : IBaseRepository<CurrencyExchange>
    {
        bool Exist(int idValue);
        IList<CurrencyExchange> Get(DateTime startDate, DateTime endDate);
    }
}
