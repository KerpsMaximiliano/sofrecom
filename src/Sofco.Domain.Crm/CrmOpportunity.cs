using System;

namespace Sofco.Domain.Crm
{
    public class CrmOpportunity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public decimal? ActualValue { get; set; }

        public Guid? ParentContactId { get; set; }

        public string ParentContactName { get; set; }
    }
}
