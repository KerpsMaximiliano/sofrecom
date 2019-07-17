using System;
using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.Rrhh
{
    public class SocialCharge : BaseEntity
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public decimal Total { get; set; }

        public IList<SocialChargeItem> Items { get; set; }
    }

    public class SocialChargeItem : BaseEntity
    {
        public string AccountName { get; set; }

        public int AccountNumber { get; set; }

        public decimal Value { get; set; }

        public int SocialChargeId { get; set; }
        public SocialCharge SocialCharge { get; set; }
    }
}
