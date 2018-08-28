using System;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlParams
    {
        public Guid? ServiceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
