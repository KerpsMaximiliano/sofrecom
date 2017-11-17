using System;

namespace Sofco.Domain.Crm
{
    public class CrmProjectHito
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Money { get; set; }

        public decimal Ammount { get; set; }

        public DateTime StartDate { get; set; }

        public int Month { get; set; }

        public bool Billed { get; set; }

        public decimal AmmountBilled { get; set; }

        public string Status { get; set; }

        public string StatusCode { get; set; }
    }
}
