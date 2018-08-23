using System;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlParams
    {
        public Guid? ServiceId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}
