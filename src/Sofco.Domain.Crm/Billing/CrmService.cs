using System;

namespace Sofco.Domain.Crm.Billing
{
    public class CrmService
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string Industry { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
