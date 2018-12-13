using System;

namespace Sofco.Domain.Crm
{
    public class CrmServiceUpdate
    {
        public Guid Id { get; set; }

        public Guid? ManagerId { get; set; }

        public string AnalyticTitle { get; set; }
    }
}
