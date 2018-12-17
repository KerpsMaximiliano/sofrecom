using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Domain.Models.Common;

namespace Sofco.Domain.Models.AdvancementAndRefund
{
    public class RefundFile
    {
        public int FileId { get; set; }
        public File File { get; set; }

        public int RefundId { get; set; }
        public Refund Refund { get; set; }
    }
}
