using System;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Common
{
    public class CurrencyExchange : BaseEntity
    {
        public DateTime Date { get; set; }

        public decimal Exchange { get; set; }

        public Currency Currency { get; set; }

        public int CurrencyId { get; set; }
    }
}
 