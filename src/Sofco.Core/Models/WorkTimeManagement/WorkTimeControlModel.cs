using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlModel
    {
        public WorkTimeResumeModel Resume { get; set; }

        public List<WorkTimeControlResourceModel> Resources { get; set; }
    }
}
