using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.Models.Rrhh
{
    public class PrepaidImportedDataModel
    {
        public IList<PrepaidImportedData> Items { get; set; }

        public IList<PrepaidTotals> Totals { get; set; }

        public bool IsClosed { get; set; }

        public IList<PrepaidImportedData> Provisioneds { get; set; }
    }

    public class PrepaidTotals
    {
        public string Prepaid { get; set; }

        public decimal TigerValue { get; set; }

        public decimal PrepaidValue { get; set; }
    }
}
