using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Common
{
    public interface ICurrencyExchangeService
    {
        Response Add(CurrencyExchangeAddModel model);

        Response Update(int id, CurrencyExchangeUpdateModel model);

        Response<IList<CurrencyExchangeModel>> Get(int startMonth, int startYear, int endMonth, int endYear);
    }
}
