using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.Common
{
    public class CurrencyExchangeAddModel
    {
        public int? Month { get; set; }

        public int? Year { get; set; }

        public decimal? Exchange { get; set; }

        public int? CurrencyId { get; set; }
    }

    public class CurrencyExchangeUpdateModel
    {
        public decimal? Exchange { get; set; }
    }

    public class CurrencyExchangeModel
    {
        public IList<CurrencyExchangeItemModel> Items { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string Description { get; set; }
    }

    public class CurrencyExchangeItemModel
    {
        public int Id { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyDesc { get; set; }

        public decimal Exchange { get; set; }
    }
}
