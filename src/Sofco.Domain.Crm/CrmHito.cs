using System;

namespace Sofco.Domain.Crm
{
    public class CrmHito
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string ManagerName { get; set; }

        public string ManagerMail { get; set; }

        public string ProjectName { get; set; }
    }
}
