using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeRejectParams
    {
        public string Comments { get; set; }

        public IList<int> HourIds { get; set; }
    }
}
