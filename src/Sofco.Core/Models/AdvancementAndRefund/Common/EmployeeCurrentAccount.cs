using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.AdvancementAndRefund.Common
{
   public class EmployeeCurrentAccount
    {
        public string Currency { get; set; }

        public decimal AdvancementTotal { get; set; }

        public decimal RefundTotal { get; set; }

        public decimal UserRefund { get; set; }

        public decimal CompanyRefund { get; set; }
    }
}
